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
using Java.Lang;


namespace MediaCodecHelper {
		
	using Android.Opengl;
	using Android.Util;
	using Android.Views;

	/**
	* Holds state associated with a Surface used for MediaCodec encoder input.
	* <p>
	* The constructor takes a Surface obtained from MediaCodec.createInputSurface(), and uses that
	* to create an EGL window surface. Calls to eglSwapBuffers() cause a frame of data to be sent
	* to the video encoder.
	*/

	public class InputSurface {
		private const string TAG = "InputSurface";
		private const bool VERBOSE = false;
		private const int EGL_RECORDABLE_ANDROID = 0x3142;
		private const int EGL_OPENGL_ES2_BIT = 4;
		private EGLDisplay _EGLDisplay;
		private EGLContext _EGLContext;
		private EGLSurface _EGLSurface;
		private Surface _surface;
		/**
	* Creates an InputSurface from a Surface.
	*/
		public InputSurface(Surface surface) {
			if (surface == null) {
				throw new NullPointerException();
			}
			_surface = surface;
			EglSetup();
		}
		/**
	* Prepares EGL. We want a GLES 2.0 context and a surface that supports recording.
	*/
		private void EglSetup() {
			_EGLDisplay = EGL14.EglGetDisplay(EGL14.EglDefaultDisplay);
			if (_EGLDisplay == EGL14.EglNoDisplay) {
				throw new RuntimeException("unable to get EGL14 display");
			}
			int[] version = new int[2];
			if (!EGL14.EglInitialize(_EGLDisplay, version, 0, version, 1)) {
				_EGLDisplay = null;
				throw new RuntimeException("unable to initialize EGL14");
			}
			// Configure EGL for pbuffer and OpenGL ES 2.0. We want enough RGB bits
			// to be able to tell if the frame is reasonable.
			int[] attribList = {
				EGL14.EglRedSize, 8,
				EGL14.EglGreenSize, 8,
				EGL14.EglBlueSize, 8,
				EGL14.EglRenderableType, EGL_OPENGL_ES2_BIT,
				EGL_RECORDABLE_ANDROID, 1,
				EGL14.EglNone
			};
			var configs = new EGLConfig[1];
			var numConfigs = new int[1];
			if (!EGL14.EglChooseConfig(_EGLDisplay, attribList, 0, configs, 0, configs.Length,
				numConfigs, 0)) {
				throw new RuntimeException("unable to find RGB888+recordable ES2 EGL config");
			}
			// Configure context for OpenGL ES 2.0.
			int[] attrib_list = {
				EGL14.EglContextClientVersion, 2,
				EGL14.EglNone
			};

			_EGLContext = EGL14.EglCreateContext(_EGLDisplay, configs[0], EGL14.EglNoContext,
				attrib_list, 0);
			CheckEglError("eglCreateContext");
			if (_EGLContext == null) {
				throw new RuntimeException("null context");
			}

			// Create a window surface, and attach it to the Surface we received.
			int[] surfaceAttribs = {
				EGL14.EglNone
			};

			_EGLSurface = EGL14.EglCreateWindowSurface(_EGLDisplay, configs[0], _surface,
				surfaceAttribs, 0);
			CheckEglError("eglCreateWindowSurface");
			if (_EGLSurface == null) {
				throw new RuntimeException("surface was null");
			}
		}
		/**
	* Discard all resources held by this class, notably the EGL context. Also releases the
	* Surface that was passed to our constructor.
	*/
		public void Release() {
			
			if (EGL14.EglGetCurrentContext().Equals(_EGLContext)) {
				// Clear the current context and surface to ensure they are discarded immediately.
				EGL14.EglMakeCurrent(_EGLDisplay, EGL14.EglNoSurface, EGL14.EglNoSurface,
					EGL14.EglNoContext);
			}

			EGL14.EglDestroySurface(_EGLDisplay, _EGLSurface);
			EGL14.EglDestroyContext(_EGLDisplay, _EGLContext);

			//EGL14.eglTerminate(mEGLDisplay);
			_surface.Release();

			// null everything out so future attempts to use this object will cause an NPE
			_EGLDisplay = null;
			_EGLContext = null;
			_EGLSurface = null;
			_surface = null;
		}

		/**
		* Makes our EGL context and surface current.
		*/
		public void MakeCurrent() {
			if (!EGL14.EglMakeCurrent(_EGLDisplay, _EGLSurface, _EGLSurface, _EGLContext)) {
				throw new RuntimeException("eglMakeCurrent failed");
			}
		}

		/**
		* Calls eglSwapBuffers. Use this to "publish" the current frame.
		*/
		public bool SwapBuffers() {
			return EGL14.EglSwapBuffers(_EGLDisplay, _EGLSurface);
		}

		/**
		* Returns the Surface that the MediaCodec receives buffers from.
		*/
		public Surface Surface {
			get { 
				return _surface;
			}
		}

		/**
		* Sends the presentation time stamp to EGL. Time is expressed in nanoseconds.
		*/
		public void SetPresentationTime(long nsecs) {
			EGLExt.EglPresentationTimeANDROID(_EGLDisplay, _EGLSurface, nsecs);
		}

		/**
		* Checks for EGL errors.
		*/
		private void CheckEglError(string msg) {
			bool failed = false;
			int error;
			while ((error = EGL14.EglGetError()) != EGL14.EglSuccess) {
				Log.Error(TAG, msg + ": EGL error: 0x" + Integer.ToHexString(error));
				failed = true;
			}

			if (failed) {
				throw new RuntimeException("EGL error encountered (see log)");
			}
		}
	}
}