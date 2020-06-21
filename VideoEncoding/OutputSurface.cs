/*
* Copyright (C) 2013 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

/**
* Holds state associated with a Surface used for MediaCodec decoder output.
* <p>
* The (width,height) constructor for this class will prepare GL, create a SurfaceTexture,
* and then create a Surface for that SurfaceTexture. The Surface can be passed to
* MediaCodec.configure() to receive decoder output. When a frame arrives, we latch the
* texture with updateTexImage, then render the texture with GL to a pbuffer.
* <p>
* The no-arg constructor skips the GL preparation step and doesn't allocate a pbuffer.
* Instead, it just creates the Surface and SurfaceTexture, and when a frame arrives
* we just draw it on whatever surface is current.
* <p>
* By default, the Surface will be using a BufferQueue in asynchronous mode, so we
* can potentially drop frames.
*/


using Android.Graphics;
using Android.Util;
using Android.Views;

using EGL14 = Android.Opengl.EGL14;
using GLES20 = Android.Opengl.GLES20;
using GLES11Ext = Android.Opengl.GLES11Ext;
using GLSurfaceView = Android.Opengl.GLSurfaceView;
using IEGL10 = Javax.Microedition.Khronos.Egl.IEGL10;
using EGL10 = Javax.Microedition.Khronos.Egl.EGL10;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using EGLContext = Javax.Microedition.Khronos.Egl.EGLContext;
using EGLDisplay = Javax.Microedition.Khronos.Egl.EGLDisplay;
using EGLSurface = Javax.Microedition.Khronos.Egl.EGLSurface;
using GL = Javax.Microedition.Khronos.Opengles.IGL;
using GL10 = Javax.Microedition.Khronos.Opengles.GL10; // IGL10?
using Java.Lang;
using Android.Runtime;

namespace MediaCodecHelper {	
	
	public class OutputSurface : Java.Lang.Object, SurfaceTexture.IOnFrameAvailableListener {
		private const string TAG = "OutputSurface";
		private const bool VERBOSE = false;
		private const int EGL_OPENGL_ES2_BIT = 4;
		private IEGL10 mEGL;
		private EGLDisplay mEGLDisplay;
		private EGLContext mEGLContext;
		private EGLSurface mEGLSurface;
		private SurfaceTexture _surfaceTexture;
		private Surface _surface;
		private object _frameSyncObject = new object(); // guards mFrameAvailable
		public bool IsFrameAvailable;
		private TextureRender _textureRender;
		/**
		* Creates an OutputSurface backed by a pbuffer with the specifed dimensions. The new
		* EGL context and surface will be made current. Creates a Surface that can be passed
		* to MediaCodec.configure().
		*/
		public OutputSurface(int width, int height) {
			if (width <= 0 || height <= 0) {
				throw new IllegalArgumentException ();
			}
			eglSetup (width, height);
			makeCurrent ();
			setup ();
		}
	/**
* Creates an OutputSurface using the current EGL context. Creates a Surface that can be
* passed to MediaCodec.configure().
*/
	public OutputSurface() {
		setup();
	}
	/**
* Creates instances of TextureRender and SurfaceTexture, and a Surface associated
* with the SurfaceTexture.
*/
	private void setup() {
		_textureRender = new TextureRender();
		_textureRender.SurfaceCreated();
		// Even if we don't access the SurfaceTexture after the constructor returns, we
		// still need to keep a reference to it. The Surface doesn't retain a reference
		// at the Java level, so if we don't either then the object can get GCed, which
		// causes the native finalizer to run.
		
		_surfaceTexture = new SurfaceTexture(_textureRender.TextureId);
		// This doesn't work if OutputSurface is created on the thread that CTS started for
		// these test cases.
		//
		// The CTS-created thread has a Looper, and the SurfaceTexture constructor will
		// create a Handler that uses it. The "frame available" message is delivered
		// there, but since we're not a Looper-based thread we'll never see it. For
		// this to do anything useful, OutputSurface must be created on a thread without
		// a Looper, so that SurfaceTexture uses the main application Looper instead.
		//
		// Java language note: passing "this" out of a constructor is generally unwise,
		// but we should be able to get away with it here.
		//_surfaceTexture.SetOnFrameAvailableListener(this);
		_surfaceTexture.FrameAvailable += FrameAvailable;
		_surface = new Surface(_surfaceTexture);

	}
	

	private void FrameAvailable (object sender, SurfaceTexture.FrameAvailableEventArgs e)
	{
		System.Threading.Monitor.Enter (_frameSyncObject);
		if (IsFrameAvailable) {
			throw new RuntimeException("mFrameAvailable already set, frame could be dropped");
		}
		IsFrameAvailable = true;
		System.Threading.Monitor.PulseAll (_frameSyncObject);
		System.Threading.Monitor.Exit (_frameSyncObject);
	}

		/**
* Prepares EGL. We want a GLES 2.0 context and a surface that supports pbuffer.
*/

	private void eglSetup(int width, int height) {
			mEGL = (IEGL10)EGLContext.EGL;
		mEGLDisplay = mEGL.EglGetDisplay(EGL10.EglDefaultDisplay);
		if (!mEGL.EglInitialize(mEGLDisplay, null)) {
			throw new RuntimeException("unable to initialize EGL10");
		}
		// Configure EGL for pbuffer and OpenGL ES 2.0. We want enough RGB bits
		// to be able to tell if the frame is reasonable.
		int[] attribList = {
				EGL10.EglRedSize, 8,
				EGL10.EglGreenSize, 8,
				EGL10.EglBlueSize, 8,
				EGL10.EglSurfaceType, EGL10.EglPbufferBit,
				EGL10.EglRenderableType, EGL_OPENGL_ES2_BIT,
				EGL10.EglNone
		};
		EGLConfig[] configs = new EGLConfig[1];
		int[] numConfigs = new int[1];
		if (!mEGL.EglChooseConfig(mEGLDisplay, attribList, configs, 1, numConfigs)) {
			throw new RuntimeException("unable to find RGB888+pbuffer EGL config");
		}
		// Configure context for OpenGL ES 2.0.
		int[] attrib_list = {
				EGL14.EglContextClientVersion, 2,
				EGL10.EglNone
		};
			mEGLContext = mEGL.EglCreateContext(mEGLDisplay, configs[0], EGL10.EglNoContext,
			attrib_list);
		CheckEglError("eglCreateContext");
		if (mEGLContext == null) {
			throw new RuntimeException("null context");
		}
		// Create a pbuffer surface. By using this for output, we can use glReadPixels
		// to test values in the output.
		int[] surfaceAttribs = {
				EGL10.EglWidth, width,
				EGL10.EglHeight, height,
				EGL10.EglNone
		};
		mEGLSurface = mEGL.EglCreatePbufferSurface(mEGLDisplay, configs[0], surfaceAttribs);
		CheckEglError("eglCreatePbufferSurface");
		if (mEGLSurface == null) {
			throw new RuntimeException("surface was null");
		}
	}
	/**
* Discard all resources held by this class, notably the EGL context.
*/
	public void Release() {
		if (mEGL != null) {
			if (mEGL.EglGetCurrentContext().Equals(mEGLContext)) {
				// Clear the current context and surface to ensure they are discarded immediately.
					mEGL.EglMakeCurrent(mEGLDisplay, EGL10.EglNoSurface, EGL10.EglNoSurface,
						EGL10.EglNoContext);
			}
			mEGL.EglDestroySurface(mEGLDisplay, mEGLSurface);
			mEGL.EglDestroyContext(mEGLDisplay, mEGLContext);
			//mEGL.eglTerminate(mEGLDisplay);
		}
		_surface.Release();
		// this causes a bunch of warnings that appear harmless but might confuse someone:
		// W BufferQueue: [unnamed-3997-2] cancelBuffer: BufferQueue has been abandoned!
		//mSurfaceTexture.release();
		// null everything out so future attempts to use this object will cause an NPE
		mEGLDisplay = null;
		mEGLContext = null;
		mEGLSurface = null;
		mEGL = null;
		_textureRender = null;
		_surface = null;
		_surfaceTexture = null;
	}
	/**
* Makes our EGL context and surface current.
*/
	public void makeCurrent() {
		if (mEGL == null) {
			throw new RuntimeException("not configured for makeCurrent");
		}
		CheckEglError("before makeCurrent");
		if (!mEGL.EglMakeCurrent(mEGLDisplay, mEGLSurface, mEGLSurface, mEGLContext)) {
			throw new RuntimeException("eglMakeCurrent failed");
		}
	}
	/**
* Returns the Surface that we draw onto.
*/
	public Surface Surface {
			get { 
				return _surface;
			}
	}

		/**
* Returns the Surface Texture
*/
		public SurfaceTexture SurfaceTexture {
			get { 
				return _surfaceTexture;
			}
		}

	/**
* Replaces the fragment shader.
*/
	public void ChangeFragmentShader(string fragmentShader) {
		_textureRender.ChangeFragmentShader(fragmentShader);
	}

		public bool AwaitNewImage(bool returnOnFailure = false) {
			
			const int TIMEOUT_MS = 1000;

			System.Threading.Monitor.Enter (_frameSyncObject);


			while (!IsFrameAvailable) {
				try {
					// Wait for onFrameAvailable() to signal us.  Use a timeout to avoid
					// stalling the test if it doesn't arrive.
					System.Threading.Monitor.Wait (_frameSyncObject, TIMEOUT_MS);

					if (!IsFrameAvailable) {
						if (returnOnFailure) {
							return false;
						}
						// TODO: if "spurious wakeup", continue while loop
						throw new RuntimeException ("frame wait timed out");
					}
				} catch (InterruptedException ie) {
					if (returnOnFailure) {
						return false;
					}
					// shouldn't happen
					throw new RuntimeException (ie);
				}
			}


			IsFrameAvailable = false;

			System.Threading.Monitor.Exit (_frameSyncObject);

			var curDisplay = EGLContext.EGL.JavaCast<IEGL10>().EglGetCurrentDisplay();
			_textureRender.CheckGlError ("before updateTexImage");
			_surfaceTexture.UpdateTexImage ();
			return true;
		}
	/**
* Draws the data from SurfaceTexture onto the current EGL surface.
*/
	public void DrawImage() {
			_textureRender.DrawFrame(_surfaceTexture);
	}
	
	public void OnFrameAvailable(SurfaceTexture st) {
		if (VERBOSE) Log.Debug(TAG, "new frame available");
		lock (_frameSyncObject) {
			if (IsFrameAvailable) {
				throw new RuntimeException("mFrameAvailable already set, frame could be dropped");
			}
			IsFrameAvailable = true;
			//_frameSyncObject.NotifyAll();
		}
	}
	/**
* Checks for EGL errors.
*/
	private void CheckEglError(string msg) {
		bool failed = false;
		int error;
			while ((error = mEGL.EglGetError()) != EGL10.EglSuccess) {
				Log.Error(TAG, msg + ": EGL error: " + error);
			failed = true;
		}
		if (failed) {
			throw new RuntimeException("EGL error encountered (see log)");
		}
	}
}
}