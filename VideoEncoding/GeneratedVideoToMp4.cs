using Android.Media;
using Android.Opengl;
using Android.Util;
using Java.IO;
using Java.Nio;
using Java.Util;
using Javax.Microedition.Khronos.Opengles;
/**
* This test has three steps:
* <ol>
* <li>Generate a video test stream.
* <li>Decode the video from the stream, rendering frames into a SurfaceTexture.
* Render the texture onto a Surface that feeds a video encoder, modifying
* the output with a fragment shader.
* <li>Decode the second video and compare it to the expected result.
* </ol><p>
* The second step is a typical scenario for video editing. We could do all this in one
* step, feeding data through multiple stages of MediaCodec, but at some point we're
* no longer exercising the code in the way we expect it to be used (and the code
* gets a bit unwieldy).
*/
using System.Collections.Generic;
using Java.Lang;
using Android.Views;
using Android.Graphics;
using Android.Content;
using BitChute.Classes;

namespace MediaCodecHelper {

	public class GeneratedVideoToMp4 {

		private string _workingDirectory;

		public GeneratedVideoToMp4(Context context) {
			_context = context;
			_workingDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		}
        string outputPath = (Android.OS.Environment.ExternalStorageDirectory.Path
          + "/download/" + "_encoderTest" + new System.Random().Next(0, 666666) + ".mp4");
        private Context _context;

		private const string TAG = "DecodeEditEncode";
		private const bool WORK_AROUND_BUGS = false; // avoid fatal codec bugs
		private const bool DEBUG_SAVE_FILE = true; // save copy of encoded movie // TODO: Made true.
		// parameters for the encoder
		private const string  MIME_TYPE = "video/avc"; // H.264 Advanced Video Coding
		private const int FRAME_RATE = 15; // 15fps
		private const int IFRAME_INTERVAL = 10; // 10 seconds between I-frames
		// movie length, in frames
		private const int NUM_FRAMES = 120; // two seconds of video
		private const int TEST_R0 = 0; // dull green background
		private const int TEST_G0 = 136;
		private const int TEST_B0 = 0;
		private const int TEST_R1 = 236; // pink; BT.601 YUV {120,160,200}
		private const int TEST_G1 = 50;
		private const int TEST_B1 = 186;
		// Replaces TextureRender.FRAGMENT_SHADER during edit; swaps green and blue channels.
		private const string  FRAGMENT_SHADER =
			"#extension GL_OES_EGL_image_external : require\n" +
			"precision mediump float;\n" +
			"varying vec2 vTextureCoord;\n" +
			"uniform samplerExternalOES sTexture;\n" +
			"void main() {\n" +
			" gl_FragColor = texture2D(sTexture, vTextureCoord).rbga;\n" +
			"}\n";
		// size of a frame, in pixels
		private int mWidth = -1;
		private int mHeight = -1;
		// bit rate, in bits per second
		private int mBitRate = -1;
		// largest color component delta seen (i.e. actual vs. expected)
		private int mLargestColorDelta;

		public void Start() {
			setParameters(1280, 720, 6000000);
			videoEditTest();
		}

		/**
	* Sets the desired frame size and bit rate.
	*/
		private void setParameters(int width, int height, int bitRate) {
			if ((width % 16) != 0 || (height % 16) != 0) {
				Log.Warn(TAG, "WARNING: width or height not multiple of 16");
			}
			mWidth = width;
			mHeight = height;
			mBitRate = bitRate;
		}
		/**
	* Tests editing of a video file with GL.
	*/
		private void videoEditTest() {
			VideoChunks sourceChunks = new VideoChunks();
			if (!generateVideoFile(sourceChunks)) {
				// No AVC codec? Fail silently.
				return;
			}
			if (DEBUG_SAVE_FILE) {
				// Save a copy to a file. We call it ".mp4", but it's actually just an elementary
				// stream, so not all video players will know what to do with it.
				string dirName = _workingDirectory;
				string fileName = "vedit1_" + mWidth + "x" + mHeight + ".264";
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                }
                sourceChunks.saveToFile(outputPath);
			}
			VideoChunks destChunks = editVideoFile(sourceChunks);
			if (DEBUG_SAVE_FILE) {
				string dirName = _workingDirectory;
				string fileName = "vedit2_" + mWidth + "x" + mHeight + ".264";
				var fullPathFile = System.IO.Path.Combine (dirName, fileName);
				if (System.IO.File.Exists (outputPath + "_v2")) {
					System.IO.File.Delete (outputPath + "_v2");
				}
				destChunks.saveToFile(System.IO.Path.Combine(outputPath + "_v2"));
			}
			checkVideoFile(destChunks);
		}

	/**
	* Generates a test video file, saving it as VideoChunks. We generate frames with GL to
	* avoid having to deal with multiple YUV formats.
	*
	* @return true on success, false on "soft" failure
	*/
		private bool generateVideoFile(VideoChunks output) {
			if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "generateVideoFile " + mWidth + "x" + mHeight);
			MediaCodec encoder = null;
			InputSurface inputSurface = null;
			try {
				MediaCodecInfo codecInfo = selectCodec(MIME_TYPE);
				if (codecInfo == null) {
					// Don't fail CTS if they don't have an AVC codec (not here, anyway).
					Log.Error(TAG, "Unable to find an appropriate codec for " + MIME_TYPE);
					return false;
				}
				if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "found codec: " + codecInfo.Name);
				// We avoid the device-specific limitations on width and height by using values that
				// are multiples of 16, which all tested devices seem to be able to handle.
				MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, mWidth, mHeight);
				// Set some properties. Failing to specify some of these can cause the MediaCodec
				// configure() call to throw an unhelpful exception.
				format.SetInteger(MediaFormat.KeyColorFormat,
					(int) MediaCodecCapabilities.Formatsurface);
				format.SetInteger(MediaFormat.KeyBitRate, mBitRate);
				format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
				format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
				if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "format: " + format);
				output.setMediaFormat(format);
				// Create a MediaCodec for the desired codec, then configure it as an encoder with
				// our desired properties.
				encoder = MediaCodec.CreateByCodecName(codecInfo.Name);
				encoder.Configure(format, null, null, MediaCodecConfigFlags.Encode);
				inputSurface = new InputSurface(encoder.CreateInputSurface());
				inputSurface.MakeCurrent();
				encoder.Start();
				generateVideoData(encoder, inputSurface, output);
			} finally {
				if (encoder != null) {
					if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "releasing encoder");
					encoder.Stop();
					encoder.Release();
					if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "released encoder");
				}
				if (inputSurface != null) {
					inputSurface.Release();
				}
			}
			return true;
		}



		/**
	* Returns the first codec capable of encoding the specified MIME type, or null if no
	* match was found.
	*/
		private static MediaCodecInfo selectCodec(string  mimeType) {
			int numCodecs = MediaCodecList.CodecCount;
			for (int i = 0; i < numCodecs; i++) {
				MediaCodecInfo codecInfo = MediaCodecList.GetCodecInfoAt(i);
				if (!codecInfo.IsEncoder) {
					continue;
				}
				var types = codecInfo.GetSupportedTypes();
				for (int j = 0; j < types.Length; j++) {
					if (types[j].ToLower() == mimeType.ToLower()) {
						return codecInfo;
					}
				}
			}
			return null;
		}

		const int TIMEOUT_USEC = 10000;

		/**
	* Generates video frames, feeds them into the encoder, and writes the output to the
	* VideoChunks instance.
	*/
		private void generateVideoData(MediaCodec encoder, InputSurface inputSurface,
			VideoChunks output) {

			ByteBuffer[] encoderOutputBuffers = encoder.GetOutputBuffers();
			MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
			int generateIndex = 0;
			int outputCount = 0;
			// Loop until the output side is done.
			bool inputDone = false;
			bool outputDone = false;
			while (!outputDone) {
				if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "gen loop");
				// If we're not done submitting frames, generate a new one and submit it. The
				// eglSwapBuffers call will block if the input is full.
				if (!inputDone) {
					if (generateIndex == NUM_FRAMES) {
						// Send an empty frame with the end-of-stream flag set.
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "signaling input EOS");
						if (WORK_AROUND_BUGS) {
							// Might drop a frame, but at least we won't crash mediaserver.
							try { Thread.Sleep(500); } catch (InterruptedException ie) {}
							outputDone = true;
						} else {
							encoder.SignalEndOfInputStream();
						}
						inputDone = true;
					} else {
						generateSurfaceFrame(generateIndex);
						inputSurface.SetPresentationTime(computePresentationTime(generateIndex) * 1000);
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "inputSurface swapBuffers");
						inputSurface.SwapBuffers();
					}
					generateIndex++;
				}
				// Check for output from the encoder. If there's no output yet, we either need to
				// provide more input, or we need to wait for the encoder to work its magic. We
				// can't actually tell which is the case, so if we can't get an output buffer right
				// away we loop around and see if it wants more input.
				//
				// If we do find output, drain it all before supplying more input.
				while (true) {
					int encoderStatus = encoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
					if (encoderStatus == (int) MediaCodecInfoState.TryAgainLater) {
						// no output available yet
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "no output from encoder available");
						break; // out of while
					} else if (encoderStatus == (int) MediaCodecInfoState.OutputBuffersChanged) {
						// not expected for an encoder
						encoderOutputBuffers = encoder.GetOutputBuffers();
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output buffers changed");
					} else if (encoderStatus == (int) MediaCodecInfoState.OutputFormatChanged) {
						// not expected for an encoder
						MediaFormat newFormat = encoder.OutputFormat;
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output format changed: " + newFormat);
					} else if (encoderStatus < 0) {
						fail("unexpected result from encoder.dequeueOutputBuffer: " + encoderStatus);
					} else { // encoderStatus >= 0
						ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
						if (encodedData == null) {
							fail("encoderOutputBuffer " + encoderStatus + " was null");
						}
						// Codec config flag must be set iff this is the first chunk of output. This
						// may not hold for all codecs, but it appears to be the case for video/avc.
						assertTrue((info.Flags & MediaCodec.BufferFlagCodecConfig) != 0 ||
							outputCount != 0);
						if (info.Size != 0) {
							// Adjust the ByteBuffer values to match BufferInfo.
							encodedData.Position(info.Offset);
							encodedData.Limit(info.Offset + info.Size);
							output.addChunk(encodedData, (int) info.Flags, info.PresentationTimeUs);
							outputCount++;
						}
						encoder.ReleaseOutputBuffer(encoderStatus, false);
						if ((info.Flags & MediaCodec.BufferFlagEndOfStream) != 0) {
							outputDone = true;
							break; // out of while
						}
					}
				}
			}


			assertEquals("Frame count", NUM_FRAMES + 1, outputCount);
		}

		void assertEquals(string tag, object a, object b) {
			Log.Debug (string.Format("{0} Should Be Equals", tag), (a.Equals (b)).ToString());
		}

		void assertTrue(bool a) {
			Log.Debug ("Should Be True", (a == true).ToString ());
		}

		void fail(string message) {
			Log.Debug ("Should Crash", message);
		}
		/**
	* Generates a frame of data using GL commands.
	* <p>
	* We have an 8-frame animation sequence that wraps around. It looks like this:
	* <pre>
	* 0 1 2 3
	* 7 6 5 4
	* </pre>
	* We draw one of the eight rectangles and leave the rest set to the zero-fill color. */
		private void generateSurfaceFrame(int frameIndex) {
			frameIndex %= 8;
			int startX, startY;
			if (frameIndex < 4) {
				// (0,0) is bottom-left in GL
				startX = frameIndex * (mWidth / 4);
				startY = mHeight / 2;
			} else {
				startX = (7 - frameIndex) * (mWidth / 4);
				startY = 0;
			}
			GLES20.GlDisable(GLES20.GlScissorTest);
			GLES20.GlClearColor(TEST_R0 / 255.0f, TEST_G0 / 255.0f, TEST_B0 / 255.0f, 1.0f);
			GLES20.GlClear(GLES20.GlColorBufferBit);
			GLES20.GlEnable(GLES20.GlScissorTest);
			GLES20.GlScissor(startX, startY, mWidth / 4, mHeight / 2);
			GLES20.GlClearColor(TEST_R1 / 255.0f, TEST_G1 / 255.0f, TEST_B1 / 255.0f, 1.0f);
			GLES20.GlClear(GLES20.GlColorBufferBit);
		}
		/**
	* Edits a video file, saving the contents to a new file. This involves decoding and
	* re-encoding, not to mention conversions between YUV and RGB, and so may be lossy.
	* <p>
	* If we recognize the decoded format we can do this in Java code using the ByteBuffer[]
	* output, but it's not practical to support all OEM formats. By using a SurfaceTexture
	* for output and a Surface for input, we can avoid issues with obscure formats and can
	* use a fragment shader to do transformations.
	*/
		private VideoChunks editVideoFile(VideoChunks inputData) {
			if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "editVideoFile " + mWidth + "x" + mHeight);
			VideoChunks outputData = new VideoChunks();
			MediaCodec decoder = null;
			MediaCodec encoder = null;
			InputSurface inputSurface = null;
			OutputSurface outputSurface = null;
			try {
				
				MediaFormat inputFormat = inputData.getMediaFormat();
				// Create an encoder format that matches the input format. (Might be able to just
				// re-use the format used to generate the video, since we want it to be the same.)
				MediaFormat outputFormat = MediaFormat.CreateVideoFormat(MIME_TYPE, mWidth, mHeight);
				outputFormat.SetInteger(MediaFormat.KeyColorFormat, (int) MediaCodecInfo.CodecCapabilities.COLORFormatSurface);
				outputFormat.SetInteger(MediaFormat.KeyBitRate, inputFormat.GetInteger(MediaFormat.KeyBitRate));
				outputFormat.SetInteger(MediaFormat.KeyFrameRate, inputFormat.GetInteger(MediaFormat.KeyFrameRate));
				outputFormat.SetInteger(MediaFormat.KeyIFrameInterval, inputFormat.GetInteger(MediaFormat.KeyIFrameInterval));
				outputData.setMediaFormat(outputFormat);
				encoder = MediaCodec.CreateEncoderByType(MIME_TYPE);
				encoder.Configure(outputFormat, null, null, MediaCodecConfigFlags.Encode);
				inputSurface = new InputSurface(encoder.CreateInputSurface());
				inputSurface.MakeCurrent();
				encoder.Start();
				// OutputSurface uses the EGL context created by InputSurface.
				decoder = MediaCodec.CreateDecoderByType(MIME_TYPE);
				outputSurface = new OutputSurface();
				outputSurface.ChangeFragmentShader(FRAGMENT_SHADER);
				decoder.Configure(inputFormat, outputSurface.Surface, null, 0);
				decoder.Start();
				editVideoData(inputData, decoder, outputSurface, inputSurface, encoder, outputData);
			} finally {
				if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "shutting down encoder, decoder");
				if (outputSurface != null) {
					outputSurface.Release();
				}
				if (inputSurface != null) {
					inputSurface.Release();
				}
				if (encoder != null) {
					encoder.Stop();
					encoder.Release();
				}
				if (decoder != null) {
					decoder.Stop();
					decoder.Release();
				}
			}
			return outputData;
		}
		/**
	* Edits a stream of video data.
	*/
		private void editVideoData(VideoChunks inputData, MediaCodec decoder,
			OutputSurface outputSurface, InputSurface inputSurface, MediaCodec encoder,
			VideoChunks outputData) {
			const int TIMEOUT_USEC = 10000;
			ByteBuffer[] decoderInputBuffers = decoder.GetInputBuffers();
			ByteBuffer[] encoderOutputBuffers = encoder.GetOutputBuffers();
			MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
			int inputChunk = 0;
			int outputCount = 0;
			bool outputDone = false;
			bool inputDone = false;
			bool decoderDone = false;
			while (!outputDone) {
				if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "edit loop");
				// Feed more data to the decoder.
				if (!inputDone) {
					int inputBufIndex = decoder.DequeueInputBuffer(TIMEOUT_USEC);
					if (inputBufIndex >= 0) {
						if (inputChunk == inputData.NumChunks) {
							// End of stream -- send empty frame with EOS flag set.
							decoder.QueueInputBuffer(inputBufIndex, 0, 0, 0L,
								MediaCodecBufferFlags.EndOfStream);
							inputDone = true;
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "sent input EOS (with zero-length frame)");
						} else {
							// Copy a chunk of input to the decoder. The first chunk should have
							// the BUFFER_FLAG_CODEC_CONFIG flag set.
							ByteBuffer inputBuf = decoderInputBuffers[inputBufIndex];
							inputBuf.Clear();
							inputData.getChunkData(inputChunk, inputBuf);
							int flags = inputData.getChunkFlags(inputChunk);
							long time = inputData.getChunkTime(inputChunk);
							decoder.QueueInputBuffer(inputBufIndex, 0, inputBuf.Position(),
								time, (MediaCodecBufferFlags) flags); // TODO: Not sure if it's MediaCodecBufferFlags, verify.
							if (AppSettings.Logging.SendToConsole) {
								Log.Debug(TAG, "submitted frame " + inputChunk + " to dec, size=" +
									inputBuf.Position() + " flags=" + flags);
							}
							inputChunk++;
						}
					} else {
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "input buffer not available");
					}
				}
				// Assume output is available. Loop until both assumptions are false.
				bool decoderOutputAvailable = !decoderDone;
				bool encoderOutputAvailable = true;
				while (decoderOutputAvailable || encoderOutputAvailable) {
					// Start by draining any pending output from the encoder. It's important to
					// do this before we try to stuff any more data in.
					int encoderStatus = encoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
					if (encoderStatus == (int) MediaCodecInfoState.TryAgainLater) {
						// no output available yet
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "no output from encoder available");
						encoderOutputAvailable = false;
					} else if (encoderStatus == (int) MediaCodecInfoState.OutputBuffersChanged) {
						encoderOutputBuffers = encoder.GetOutputBuffers();
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output buffers changed");
					} else if (encoderStatus == (int) MediaCodecInfoState.OutputFormatChanged) {
						MediaFormat newFormat = encoder.OutputFormat;
						if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output format changed: " + newFormat);
					} else if (encoderStatus < 0) {
						fail("unexpected result from encoder.dequeueOutputBuffer: " + encoderStatus);
					} else { // encoderStatus >= 0
						ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
						if (encodedData == null) {
							fail("encoderOutputBuffer " + encoderStatus + " was null");
						}
						// Write the data to the output "file".
						if (info.Size != 0) {
							encodedData.Position(info.Offset);
							encodedData.Limit(info.Offset + info.Size);
							outputData.addChunk(encodedData, (int) info.Flags, info.PresentationTimeUs);
							outputCount++;
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output " + info.Size + " bytes");
						}
						outputDone = (info.Flags & MediaCodec.BufferFlagEndOfStream) != 0;
						encoder.ReleaseOutputBuffer(encoderStatus, false);
					}
					if (encoderStatus != (int) MediaCodec.InfoTryAgainLater) {
						// Continue attempts to drain output.
						continue;
					}
					// Encoder is drained, check to see if we've got a new frame of output from
					// the decoder. (The output is going to a Surface, rather than a ByteBuffer,
					// but we still get information through BufferInfo.)
					if (!decoderDone) {
						int decoderStatus = decoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
						if (decoderStatus == (int) MediaCodec.InfoTryAgainLater) {
							// no output available yet
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "no output from decoder available");
							decoderOutputAvailable = false;
						} else if (decoderStatus == (int) MediaCodec.InfoOutputBuffersChanged) {
							//decoderOutputBuffers = decoder.getOutputBuffers();
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "decoder output buffers changed (we don't care)");
						} else if (decoderStatus == (int) MediaCodec.InfoOutputFormatChanged) {
							// expected before first buffer of data
							MediaFormat newFormat = decoder.OutputFormat;
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "decoder output format changed: " + newFormat);
						} else if (decoderStatus < 0) {
							fail("unexpected result from decoder.dequeueOutputBuffer: "+decoderStatus);
						} else { // decoderStatus >= 0
							if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "surface decoder given buffer " + decoderStatus + " (size=" + info.Size  +"(");
								// The ByteBuffers are null references, but we still get a nonzero
								// size for the decoded data.
								bool doRender = (info.Size != 0);
								// As soon as we call releaseOutputBuffer, the buffer will be forwarded
								// to SurfaceTexture to convert to a texture. The API doesn't
								// guarantee that the texture will be available before the call
								// returns, so we need to wait for the onFrameAvailable callback to
								// fire. If we don't wait, we risk rendering from the previous frame.
								decoder.ReleaseOutputBuffer(decoderStatus, doRender);
								if (doRender) {
									// This waits for the image and renders it after it arrives.
									if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "awaiting frame");
									outputSurface.AwaitNewImage();
									outputSurface.DrawImage();
									// Send it to the encoder.
									inputSurface.SetPresentationTime(info.PresentationTimeUs * 1000);
									if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "swapBuffers");
									inputSurface.SwapBuffers();
								}
								if ((info.Flags & MediaCodec.BufferFlagEndOfStream) != 0) {
									// forward decoder EOS to encoder
									if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "signaling input EOS");
									if (WORK_AROUND_BUGS) {
										// Bail early, possibly dropping a frame.
										return;
									} else {
										encoder.SignalEndOfInputStream();
									}
								}
								}
								}
								}
								}
								if (inputChunk != outputCount) {
									throw new RuntimeException("frame lost: " + inputChunk + " in, " +
										outputCount + " out");
								}
								}
								/**
	* Checks the video file to see if the contents match our expectations. We decode the
	* video to a Surface and check the pixels with GL.
	*/
								private void checkVideoFile(VideoChunks inputData) {
									OutputSurface surface = null;
									MediaCodec decoder = null;
									mLargestColorDelta = -1;
									if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "checkVideoFile");
									try {
										surface = new OutputSurface(mWidth, mHeight);
										MediaFormat format = inputData.getMediaFormat();
										decoder = MediaCodec.CreateDecoderByType(MIME_TYPE);
										decoder.Configure(format, surface.Surface, null, 0);
										decoder.Start();
										int badFrames = checkVideoData(inputData, decoder, surface);
										if (badFrames != 0) {
											fail("Found " + badFrames + " bad frames");
										}
									} finally {
										if (surface != null) {
											surface.Release();
										}
										if (decoder != null) {
											decoder.Stop();
											decoder.Release();
										}
				Log.Info(TAG, "Largest color delta: " + mLargestColorDelta);
									}
								}
								/**
	* Checks the video data.
	*
	* @return the number of bad frames
	*/
								private int checkVideoData(VideoChunks inputData, MediaCodec decoder, OutputSurface surface) {
									const int TIMEOUT_USEC = 1000;
									ByteBuffer[] decoderInputBuffers = decoder.GetInputBuffers();
									ByteBuffer[] decoderOutputBuffers = decoder.GetOutputBuffers();
									MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
									int inputChunk = 0;
									int checkIndex = 0;
									int badFrames = 0;
									bool outputDone = false;
									bool inputDone = false;
									while (!outputDone) {
										if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "check loop");
										// Feed more data to the decoder.
										if (!inputDone) {
											int inputBufIndex = decoder.DequeueInputBuffer(TIMEOUT_USEC);
											if (inputBufIndex >= 0) {
												if (inputChunk == inputData.NumChunks) {
													// End of stream -- send empty frame with EOS flag set.
													decoder.QueueInputBuffer(inputBufIndex, 0, 0, 0L,
								MediaCodec.BufferFlagEndOfStream);
													inputDone = true;
													if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "sent input EOS");
												} else {
													// Copy a chunk of input to the decoder. The first chunk should have
													// the BUFFER_FLAG_CODEC_CONFIG flag set.
													ByteBuffer inputBuf = decoderInputBuffers[inputBufIndex];
													inputBuf.Clear();
													inputData.getChunkData(inputChunk, inputBuf);
													int flags = inputData.getChunkFlags(inputChunk);
													long time = inputData.getChunkTime(inputChunk);
													decoder.QueueInputBuffer(inputBufIndex, 0, inputBuf.Position(),
								time, (MediaCodecBufferFlags) flags);
													if (AppSettings.Logging.SendToConsole) {
														Log.Debug(TAG, "submitted frame " + inputChunk + " to dec, size=" +
															inputBuf.Position() + " flags=" + flags);
													}
													inputChunk++;
												}
											} else {
												if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "input buffer not available");
											}
										}
										if (!outputDone) {
											int decoderStatus = decoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
					if (decoderStatus == (int) MediaCodec.InfoTryAgainLater) {
												// no output available yet
												if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "no output from decoder available");
					} else if (decoderStatus == (int) MediaCodec.InfoOutputBuffersChanged) {
												decoderOutputBuffers = decoder.GetOutputBuffers();
												if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "decoder output buffers changed");
					} else if (decoderStatus == (int) MediaCodec.InfoOutputFormatChanged) {
						MediaFormat newFormat = decoder.OutputFormat;
												if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "decoder output format changed: " + newFormat);
											} else if (decoderStatus < 0) {
												fail("unexpected result from decoder.dequeueOutputBuffer: " + decoderStatus);
											} else { // decoderStatus >= 0
												ByteBuffer decodedData = decoderOutputBuffers[decoderStatus];
												if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "surface decoder given buffer " + decoderStatus +
													" (size=" + info.Size + ")");
						if ((info.Flags & MediaCodec.BufferFlagEndOfStream) != 0) {
													if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "output EOS");
													outputDone = true;
												}
												bool doRender = (info.Size != 0);
												// As soon as we call releaseOutputBuffer, the buffer will be forwarded
												// to SurfaceTexture to convert to a texture. The API doesn't guarantee
												// that the texture will be available before the call returns, so we
												// need to wait for the onFrameAvailable callback to fire.
												decoder.ReleaseOutputBuffer(decoderStatus, doRender);
												if (doRender) {
													if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "awaiting frame " + checkIndex);
													assertEquals("Wrong time stamp", computePresentationTime(checkIndex),
														info.PresentationTimeUs);
													surface.AwaitNewImage();
													surface.DrawImage();
													if (!checkSurfaceFrame(checkIndex++)) {
														badFrames++;
													}
												}
											}
										}
									}
									return badFrames;
								}
								/**
	* Checks the frame for correctness, using GL to check RGB values.
	*
	* @return true if the frame looks good
	*/
								private bool checkSurfaceFrame(int frameIndex) {
									ByteBuffer pixelBuf = ByteBuffer.AllocateDirect(4); // TODO - reuse this
									bool frameFailed = false;
									for (int i = 0; i < 8; i++) {
										// Note the coordinates are inverted on the Y-axis in GL.
										int x, y;
										if (i < 4) {
											x = i * (mWidth / 4) + (mWidth / 8);
											y = (mHeight * 3) / 4;
										} else {
											x = (7 - i) * (mWidth / 4) + (mWidth / 8);
											y = mHeight / 4;
										}
				GLES20.GlReadPixels(x, y, 1, 1, GL10.GlRgba, GL10.GlUnsignedByte, pixelBuf);
										int r = pixelBuf.Get(0) & 0xff;
										int g = pixelBuf.Get(1) & 0xff;
										int b = pixelBuf.Get(2) & 0xff;
										//Log.Debug(TAG, "GOT(" + frameIndex + "/" + i + "): r=" + r + " g=" + g + " b=" + b);
										int expR, expG, expB;
										if (i == frameIndex % 8) {
											// colored rect (green/blue swapped)
											expR = TEST_R1;
											expG = TEST_B1;
											expB = TEST_G1;
										} else {
											// zero background color (green/blue swapped)
											expR = TEST_R0;
											expG = TEST_B0;
											expB = TEST_G0;
										}
										if (!isColorClose(r, expR) ||
											!isColorClose(g, expG) ||
											!isColorClose(b, expB)) {
											Log.Warn(TAG, "Bad frame " + frameIndex + " (rect=" + i + ": rgb=" + r +
												"," + g + "," + b + " vs. expected " + expR + "," + expG +
												"," + expB + ")");
											frameFailed = true;
										}
									}
									return !frameFailed;
								}
								/**
	* Returns true if the actual color value is close to the expected color value. Updates
	* mLargestColorDelta.
	*/
								bool isColorClose(int actual, int expected) {
									int MAX_DELTA = 8;
									int delta = Math.Abs(actual - expected);
									if (delta > mLargestColorDelta) {
										mLargestColorDelta = delta;
									}
									return (delta <= MAX_DELTA);
								}
								/**
	* Generates the presentation time for frame N, in microseconds.
	*/
								private static long computePresentationTime(int frameIndex) {
									return 123 + frameIndex * 1000000 / FRAME_RATE;
								}
								/**
	* The elementary stream coming out of the "video/avc" encoder needs to be fed back into
	* the decoder one chunk at a time. If we just wrote the data to a file, we would lose
	* the information about chunk boundaries. This class stores the encoded data in memory,
	* retaining the chunk organization.
	*/
								private class VideoChunks {
									private MediaFormat mMediaFormat;
			private List<byte[]> _chunks = new List<byte[]>();
									private List<int> mFlags = new List<int>();
									private List<long> mTimes = new List<long>();
									/**
	* Sets the MediaFormat, for the benefit of a future decoder.
	*/
									public void setMediaFormat(MediaFormat format) {
										mMediaFormat = format;
									}
									/**
	* Gets the MediaFormat that was used by the encoder.
	*/
									public MediaFormat getMediaFormat() {
										return mMediaFormat;
									}
									/**
	* Adds a new chunk. Advances buf.position to buf.limit.
	*/
									public void addChunk(ByteBuffer buf, int flags, long time) {
				byte[] data = new byte[buf.Remaining ()];
				buf.Get (data);
				_chunks.Add (data);
				mFlags.Add (flags);
				mTimes.Add (time);
			}
									/**
	* Returns the number of chunks currently held.
	*/
			public int NumChunks {
				get { 
					return _chunks.Count;
				}
			}
									/**
	* Copies the data from chunk N into "dest". Advances dest.position.
	*/
									public void getChunkData(int chunk, ByteBuffer dest) {
				byte[] data = _chunks [chunk];
				dest.Put (data);
			}
									/**
	* Returns the flags associated with chunk N.
	*/
									public int getChunkFlags(int chunk) {
				return mFlags [chunk];
									}
									/**
	* Returns the timestamp associated with chunk N.
	*/
									public long getChunkTime(int chunk) {
				return mTimes [chunk];
									}
									/**
	* Writes the chunks to a file as a contiguous stream. Useful for debugging.
	*/
			public void saveToFile(string path) {
				System.IO.FileStream fos = null;
				BufferedOutputStream bos = null;

				try {
					fos = new System.IO.FileStream(path, System.IO.FileMode.Create);
					bos = new BufferedOutputStream(fos);
					fos = null;     // closing bos will also close fos

					int numChunks = NumChunks;
					for (int i = 0; i < numChunks; i++) {
						byte[] chunk = _chunks[i];
						bos.Write(chunk);
					}
				} catch (System.IO.IOException) {
					//throw new RuntimeException(ioe);
				} finally {
					try {
						if (bos != null) {
							bos.Close();
						}
						if (fos != null) {
							fos.Close();
						}
					} catch (System.IO.IOException) {
						//throw new RuntimeException(ioe);
					}
				}
			}
		}
	}
}