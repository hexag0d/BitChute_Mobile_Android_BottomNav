
using System.Collections.Generic;
using static BitChute.Models.CreatorModel;

namespace BitChute.Models
{
    public class CommentModel
    {
        public class Comment
        {
            public Comment()
            {

            }

            public Comment(string commenterName, string commentText, 
                List<object> commentContents, int likes, int dislikes, 
                List<Comment> replies, Creator creator, 
                Android.Graphics.Drawables.Drawable drawableAvatar, 
                Android.Graphics.Bitmap bitmapAvatar)
            {
                CommenterName = commenterName;
                CommentText = commentText;
                Likes = likes;
                Dislikes = dislikes;
                if (replies != null)
                Replies = replies;

                if (commentContents != null)
                {

                }

                if (creator != null)
                {
                    Creator = creator;
                }
                if (bitmapAvatar != null)
                {
                    CommenterAvatarBitmap = bitmapAvatar;
                }
                else 
                {
                    if (drawableAvatar != null)
                    {
                        CommenterAvatarDrawable = drawableAvatar;
                    }
                    else
                    {
                        //CommenterAvatarDrawable = 
                    }
                }

            }

            public string CommenterName { get; set; }
            public string CommentText { get; set; }
            public Android.Graphics.Drawables.Drawable CommenterAvatarDrawable { get; set; }
            public Android.Graphics.Bitmap CommenterAvatarBitmap { get; set; }

            public int Likes { get; set; }
            public int Dislikes { get; set; }

            public List<Comment> Replies { get; set; }

            /// <summary>
            /// this would include images, links, other potential contents
            /// </summary>
            public List<object> CommentContents { get; set; }

            //if the user has a channel then we set this
            //this will allow viewer to click the avatar and navigate to user
            //channel
            public Creator Creator{ get; set; }

        }

        public static int GetBlankAvatarInt()
        {
            return Resource.Drawable.noavatar;
        }

        public class SampleCommentList
        {
            Comment comment = new Comment("", "", null, 0, 0, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null);

            public static List<Comment> GetSampleCommentList()
            {
                List<Comment> sampleComments = new List<Comment>{
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null), 
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(),  MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null)
                };

                return sampleComments;
            }
        }
    }
}