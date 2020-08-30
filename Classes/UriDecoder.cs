
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BitChute.Classes
{
    class UriDecoder
    {
        public static UriDecoder StatDecoder = new UriDecoder();
        public UriDecoder() { }

        public static string ConvertUriToString(Android.Net.Uri uri)
        {
            string fp = "";
            //fp = uri.EncodedSchemeSpecificPart.Replace(@"//com.android.externalstorage.documents/document/primary%3A", FileBrowser.WorkingDirectory);

            fp = uri.EncodedSchemeSpecificPart.Replace(@"%3A", @"/").Replace(@"%2F", @"/");
            fp = fp.Replace(@"//com.android.externalstorage.documents/document/primary/", FileBrowser.WorkingDirectory);
            //"//com.android.externalstorage.documents/document/primary%3ADownload%2Fcar_audio_sample.mp4"
            return fp;
        }

        public static System.String GetFileNameByUri(Context context, Uri uri)
        {
            System.String fileName = "unknown";//default fileName
            Uri filePathUri = uri;
            if (uri.Scheme.ToString().CompareTo("content") == 0)
            {
                ICursor cursor = context.ContentResolver.Query(uri, null, null, null, null);
                if (cursor.MoveToFirst())
                {
                    //int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);//Instead of "MediaStore.Images.Media.DATA" can be used "_data"
                    int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Data);//Instead of "MediaStore.Images.Media.DATA" can be used "_data"

                    filePathUri = Uri.Parse(cursor.GetString(column_index));
                    fileName = filePathUri.LastPathSegment.ToString();
                }
            }
            else if (uri.Scheme.CompareTo("file") == 0)
            {
                fileName = filePathUri.LastPathSegment.ToString();
            }
            else
            {
                fileName = fileName + "_" + filePathUri.LastPathSegment;
            }
            return fileName;
        }

        public ContentResolver GetContentResolver(Context context = null)
        {
            if (context == null) { context = Android.App.Application.Context; }
            return context.ContentResolver;
        }

        /* Get uri related content real local file path. */
        public static System.String getUriRealPath(Context ctx, Uri uri)
        {
            System.String ret = "";

            if (StatDecoder.isAboveKitKat())
            {
                // Android OS above sdk version 19.
                ret = StatDecoder.getUriRealPathAboveKitkat(ctx, uri);
            }
            else
            {
                // Android OS below sdk version 19
                ret = StatDecoder.getImageRealPath(StatDecoder.GetContentResolver(ctx), uri, null);
            }

            return ret;
        }

        private System.String getUriRealPathAboveKitkat(Context ctx, Uri uri)
        {
            System.String ret = "";

            if (ctx != null && uri != null)
            {
                
                if (isContentUri(uri))
                {
                    if (isGooglePhotoDoc(uri.Authority))
                    {
                        ret = uri.LastPathSegment;
                    }
                    else
                    {
                        ret = getImageRealPath(GetContentResolver(ctx), uri, null);
                    }
                }
                else if (isFileUri(uri))
                {
                    ret = uri.Path;
                }
                else if (isDocumentUri(ctx, uri))
                {

                    // Get uri related document id.
                    System.String documentId = DocumentsContract.GetDocumentId(uri);

                    // Get uri authority.
                    System.String uriAuthority = uri.Authority;

                    if (isMediaDoc(uriAuthority))
                    {
                        var idArr = documentId.Split(":");
                        if (idArr.Length == 2)
                        {
                            // First item is document type.
                            System.String docType = idArr[0];

                            // Second item is document real id.
                            System.String realDocId = idArr[1];

                            // Get content uri by document type.
                            Uri mediaContentUri = MediaStore.Images.Media.ExternalContentUri;
                            if ("image".Equals(docType))
                            {
                                mediaContentUri = MediaStore.Images.Media.ExternalContentUri;
                            }
                            else if ("video".Equals(docType))
                            {
                                mediaContentUri = MediaStore.Video.Media.ExternalContentUri;
                            }
                            else if ("audio".Equals(docType))
                            {
                                mediaContentUri = MediaStore.Audio.Media.ExternalContentUri;
                            }

                            // Get where clause with real document id.
                            System.String whereClause = MediaStore.Images.Media.InterfaceConsts.Id + " = " + realDocId;

                            ret = getImageRealPath(GetContentResolver(ctx), mediaContentUri, whereClause);
                        }

                    }
                    else if (isDownloadDoc(uriAuthority))
                    {
                        // Build download uri.
                        Uri downloadUri = Uri.Parse("content://downloads/public_downloads");

                        // Append download document id at uri end.
                        Uri downloadUriAppendId = ContentUris.WithAppendedId(downloadUri, long.Parse(documentId));

                        ret = getImageRealPath(GetContentResolver(ctx), downloadUriAppendId, null);

                    }
                    else if (isExternalStoreDoc(uriAuthority))
                    {
                        var idArr = documentId.Split(":");
                        if (idArr.Length == 2)
                        {
                            System.String type = idArr[0];
                            System.String realDocId = idArr[1];

                            if ("primary".Equals(type.ToLower()))
                            {
                                ret = Environment.ExternalStorageDirectory + "/" + realDocId;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /* Check whether current android os version is bigger than kitkat or not. */
        private bool isAboveKitKat()
        {
            bool ret = false;
            ret = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;
            return ret;
        }

        /* Check whether this uri represent a document or not. */
        private bool isDocumentUri(Context ctx, Uri uri)
        {
            bool ret = false;
            if (ctx != null && uri != null)
            {
                ret = DocumentsContract.IsDocumentUri(ctx, uri);
            }
            return ret;
        }

        /* Check whether this uri is a content uri or not.
        *  content uri like content://media/external/images/media/1302716
        *  */
        private bool isContentUri(Uri uri)
        {
            bool ret = false;
            if (uri != null)
            {
                System.String uriSchema = uri.Scheme;
                if ("content".Equals(uriSchema.ToLower()))
                {
                    ret = true;
                }
            }
            return ret;
        }

        /* Check whether this uri is a file uri or not.
        *  file uri like file:///storage/41B7-12F1/DCIM/Camera/IMG_20180211_095139.jpg
        * */
        private bool isFileUri(Uri uri)
        {
            bool ret = false;
            if (uri != null)
            {
                System.String uriSchema = uri.Scheme;
                if ("file".Equals(uriSchema.ToLower()))
                {
                    ret = true;
                }
            }
            return ret;
        }


        /* Check whether this document is provided by ExternalStorageProvider. */
        private bool isExternalStoreDoc(System.String uriAuthority)
        {
            bool ret = false;

            if ("com.android.externalstorage.documents".Equals(uriAuthority))
            {
                ret = true;
            }

            return ret;
        }

        /* Check whether this document is provided by DownloadsProvider. */
        private bool isDownloadDoc(System.String uriAuthority)
        {
            bool ret = false;

            if ("com.android.providers.downloads.documents".Equals(uriAuthority))
            {
                ret = true;
            }

            return ret;
        }

        /* Check whether this document is provided by MediaProvider. */
        private bool isMediaDoc(System.String uriAuthority)
        {
            bool ret = false;

            if ("com.android.providers.media.documents".Equals(uriAuthority))
            {
                ret = true;
            }

            return ret;
        }

        /* Check whether this document is provided by google photos. */
        private bool isGooglePhotoDoc(System.String uriAuthority)
        {
            bool ret = false;

            if ("com.google.android.apps.photos.content".Equals(uriAuthority))
            {
                ret = true;
            }

            return ret;
        }

        /* Return uri represented document file real local path.*/
        private System.String getImageRealPath(ContentResolver contentResolver, Android.Net.Uri uri, System.String whereClause)
        {
            System.String ret = "";

            // Query the uri with condition.
            ICursor cursor = contentResolver.Query(uri, null, whereClause, null, null);

            if (cursor != null)
            {
                bool moveToFirst = cursor.MoveToFirst();
                if (moveToFirst)
                {

                    // Get columns name by uri type.
                    System.String columnName = MediaStore.Images.Media.InterfaceConsts.Data;

                    if (uri == MediaStore.Images.Media.ExternalContentUri)
                    {
                        columnName = MediaStore.Images.Media.InterfaceConsts.Data;
                    }
                    else if (uri == MediaStore.Audio.Media.ExternalContentUri)
                    {
                        columnName = MediaStore.Audio.Media.InterfaceConsts.Data;
                    }
                    else if (uri == MediaStore.Video.Media.ExternalContentUri)
                    {
                        columnName = MediaStore.Video.Media.InterfaceConsts.Data;
                    }

                    // Get column index.
                    int imageColumnIndex = cursor.GetColumnIndex(columnName);

                    // Get column value which is the uri related file local path.
                    ret = cursor.GetString(imageColumnIndex);
                }
            }

            return ret;
        }
    }
}