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

using Java.Nio;
using Android.Graphics;
using Android.Util;
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

namespace MediaCodecHelper {

	/**
	* Code for rendering a texture onto a surface using OpenGL ES 2.0.
	*/
	public class TextureRender {
		private const string TAG = "TextureRender";
		private const int FLOAT_SIZE_BYTES = 4;
		private const int TRIANGLE_VERTICES_DATA_STRIDE_BYTES = 5 * FLOAT_SIZE_BYTES;
		private const int TRIANGLE_VERTICES_DATA_POS_OFFSET = 0;
		private const int TRIANGLE_VERTICES_DATA_UV_OFFSET = 3;
		private float[] mTriangleVerticesData = {
			// X, Y, Z, U, V
			-1.0f, -1.0f, 0, 0.0f, 0.0f,
			1.0f, -1.0f, 0, 1.0f, 0.0f,
			-1.0f, 1.0f, 0, 0.0f, 1.0f,
			1.0f, 1.0f, 0, 1.0f, 1.0f,
		};
		private FloatBuffer mTriangleVertices;
		private const string VERTEX_SHADER =
			"uniform mat4 uMVPMatrix;\n" +
			"uniform mat4 uSTMatrix;\n" +
			"attribute vec4 aPosition;\n" +
			"attribute vec4 aTextureCoord;\n" +
			"varying vec2 vTextureCoord;\n" +
			"void main() {\n" +
			" gl_Position = uMVPMatrix * aPosition;\n" +
			" vTextureCoord = (uSTMatrix * aTextureCoord).xy;\n" +
			"}\n";
		private const string FRAGMENT_SHADER =
			"#extension GL_OES_EGL_image_external : require\n" +
			"precision mediump float;\n" + // highp here doesn't seem to matter
			"varying vec2 vTextureCoord;\n" +
			"uniform samplerExternalOES sTexture;\n" +
			"void main() {\n" +
			" gl_FragColor = texture2D(sTexture, vTextureCoord);\n" +
			"}\n";
		private float[] mMVPMatrix = new float[16];
		private float[] mSTMatrix = new float[16];
		private int mProgram;
		private int _textureID = -12345;
		private int muMVPMatrixHandle;
		private int muSTMatrixHandle;
		private int maPositionHandle;
		private int maTextureHandle;

		public TextureRender() {
			mTriangleVertices = ByteBuffer.AllocateDirect(
				mTriangleVerticesData.Length * FLOAT_SIZE_BYTES)
				.Order(ByteOrder.NativeOrder()).AsFloatBuffer();
			mTriangleVertices.Put(mTriangleVerticesData).Position(0);

			Android.Opengl.Matrix.SetIdentityM(mSTMatrix, 0);
		}
		public int TextureId {
			get { 
				return _textureID;
			}
		}
		public void DrawFrame(SurfaceTexture st) {
			CheckGlError("onDrawFrame start");
			st.GetTransformMatrix(mSTMatrix);
			GLES20.GlClearColor(0.0f, 1.0f, 0.0f, 1.0f);
			GLES20.GlClear(GLES20.GlDepthBufferBit | GLES20.GlColorBufferBit);
			GLES20.GlUseProgram(mProgram);
			CheckGlError("glUseProgram");
			GLES20.GlActiveTexture(GLES20.GlTexture0);
			GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, _textureID);
			mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_POS_OFFSET);
			GLES20.GlVertexAttribPointer(maPositionHandle, 3, GLES20.GlFloat, false,
				TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
			CheckGlError("glVertexAttribPointer maPosition");
			GLES20.GlEnableVertexAttribArray(maPositionHandle);
			CheckGlError("glEnableVertexAttribArray maPositionHandle");
			mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_UV_OFFSET);
			GLES20.GlVertexAttribPointer(maTextureHandle, 2, GLES20.GlFloat, false,
				TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
			CheckGlError("glVertexAttribPointer maTextureHandle");
			GLES20.GlEnableVertexAttribArray(maTextureHandle);
			CheckGlError("glEnableVertexAttribArray maTextureHandle");
			Android.Opengl.Matrix.SetIdentityM(mMVPMatrix, 0);
			GLES20.GlUniformMatrix4fv(muMVPMatrixHandle, 1, false, mMVPMatrix, 0);
			GLES20.GlUniformMatrix4fv(muSTMatrixHandle, 1, false, mSTMatrix, 0);
			GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);
			CheckGlError("glDrawArrays");
			GLES20.GlFinish();
		}
		/**
	* Initializes GL state. Call this after the EGL surface has been created and made current.
	*/
		public void SurfaceCreated() {
			mProgram = createProgram(VERTEX_SHADER, FRAGMENT_SHADER);
			if (mProgram == 0) {
				throw new RuntimeException("failed creating program");
			}
			maPositionHandle = GLES20.GlGetAttribLocation(mProgram, "aPosition");
			CheckGlError("glGetAttribLocation aPosition");
			if (maPositionHandle == -1) {
				throw new RuntimeException("Could not get attrib location for aPosition");
			}
			maTextureHandle = GLES20.GlGetAttribLocation(mProgram, "aTextureCoord");
			CheckGlError("glGetAttribLocation aTextureCoord");
			if (maTextureHandle == -1) {
				throw new RuntimeException("Could not get attrib location for aTextureCoord");
			}
			muMVPMatrixHandle = GLES20.GlGetUniformLocation(mProgram, "uMVPMatrix");
			CheckGlError("glGetUniformLocation uMVPMatrix");
			if (muMVPMatrixHandle == -1) {
				throw new RuntimeException("Could not get attrib location for uMVPMatrix");
			}
			muSTMatrixHandle = GLES20.GlGetUniformLocation(mProgram, "uSTMatrix");
			CheckGlError("glGetUniformLocation uSTMatrix");
			if (muSTMatrixHandle == -1) {
				throw new RuntimeException("Could not get attrib location for uSTMatrix");
			}
			int[] textures = new int[1];
			GLES20.GlGenTextures(1, textures, 0);
			_textureID = textures[0];
			GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, _textureID);
			CheckGlError("glBindTexture mTextureID");
			GLES20.GlTexParameterf(GLES11Ext.GlTextureExternalOes, GLES20.GlTextureMinFilter,
				GLES20.GlNearest);
			GLES20.GlTexParameterf(GLES11Ext.GlTextureExternalOes, GLES20.GlTextureMagFilter,
				GLES20.GlLinear);
			GLES20.GlTexParameteri(GLES11Ext.GlTextureExternalOes, GLES20.GlTextureWrapS,
				GLES20.GlClampToEdge);
			GLES20.GlTexParameteri(GLES11Ext.GlTextureExternalOes, GLES20.GlTextureWrapT,
				GLES20.GlClampToEdge);
			CheckGlError("glTexParameter");
		}
		/**
	* Replaces the fragment shader.
	*/
		public void ChangeFragmentShader(string fragmentShader) {
			GLES20.GlDeleteProgram(mProgram);
			mProgram = createProgram(VERTEX_SHADER, fragmentShader);
			if (mProgram == 0) {
				throw new RuntimeException("failed creating program");
			}
		}
		private int LoadShader(int shaderType, string source) {
			int shader = GLES20.GlCreateShader(shaderType);
			CheckGlError("glCreateShader type=" + shaderType);
			GLES20.GlShaderSource(shader, source);
			GLES20.GlCompileShader(shader);
			int[] compiled = new int[1];
			GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compiled, 0);
			if (compiled[0] == 0) {
				Log.Error(TAG, "Could not compile shader " + shaderType + ":");
				Log.Error(TAG, " " + GLES20.GlGetShaderInfoLog(shader));
				GLES20.GlDeleteShader(shader);
				shader = 0;
			}
			return shader;
		}
		private int createProgram(string vertexSource, string fragmentSource) {
			int vertexShader = LoadShader(GLES20.GlVertexShader, vertexSource);
			if (vertexShader == 0) {
				return 0;
			}
			int pixelShader = LoadShader(GLES20.GlFragmentShader, fragmentSource);
			if (pixelShader == 0) {
				return 0;
			}
			int program = GLES20.GlCreateProgram();
			CheckGlError("glCreateProgram");
			if (program == 0) {
				Log.Error(TAG, "Could not create program");
			}
			GLES20.GlAttachShader(program, vertexShader);
			CheckGlError("glAttachShader");
			GLES20.GlAttachShader(program, pixelShader);
			CheckGlError("glAttachShader");
			GLES20.GlLinkProgram(program);
			int[] linkStatus = new int[1];
			GLES20.GlGetProgramiv(program, GLES20.GlLinkStatus, linkStatus, 0);
			if (linkStatus[0] != GLES20.GlTrue) {
				Log.Error(TAG, "Could not link program: ");
				Log.Error(TAG, GLES20.GlGetProgramInfoLog(program));
				GLES20.GlDeleteProgram(program);
				program = 0;
			}
			return program;
		}
		public void CheckGlError
		(string op) {
			int error;
			while ((error = GLES20.GlGetError()) != GLES20.GlNoError) {
				Log.Error(TAG, op + ": glError " + error);
				throw new RuntimeException(op + ": glError " + error);
			}
		}
	}
}