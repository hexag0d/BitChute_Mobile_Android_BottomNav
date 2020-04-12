
using System;
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

                CommentId = "dummyID123456";

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
            public string CommentId { get; set; }

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
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.hex), null),
                    new Comment("cynical soapbox", "This is a sample comment; This video rocks!", null, 6, 3, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable._i51), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable._i57), null),
                    new Comment("idream", ReallyLongSamplePlainText , null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable._i62), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable._i52), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable._i70), null),
                    new Comment("Jeebus", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable._i74), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("hexagod", ReallyLongSamplePlainText , null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video rocks!", null, 6, 3, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("canis cortex", "This is a sample comment; This video rocks!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.hex), null),
                    new Comment("Commenter Name", ReallyLongSamplePlainText , null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable._i85), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, null, MainActivity.UniversalGetDrawable(Resource.Drawable.hex), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(), MainActivity.UniversalGetDrawable(Resource.Drawable.noavatar), null),
                    new Comment("Commenter Name", "This is a sample comment; This video blows!", null, 6, 3, null, GetSampleCreator(),  MainActivity.UniversalGetDrawable(Resource.Drawable._i77), null)
                };

                return sampleComments;
            }

            public static string ReallyLongSamplePlainText = 
                "Some initial thought goes into the persons mind which decides his choice, " + "\r\n" +
                "it can be favorite color , lucky color and so on. In other words some" + "\r\n" +
                " initial trigger which we term in RANDOM as SEED.This SEED is the " + "\r\n" +
                "beginning point, the trigger which instigates him to select the " + "\r\n" +
                "RANDOM value.Now if a SEED is easy to guess then those kind of " + "\r\n" +
                "random numbers are termed as PSEUDO and when a seed is " + "\r\n" +
                "difficult to guess those random numbers are termed SECURED random numbers." + "\r\n" +
                "For example a person chooses is color depending on weather and sound" + "\r\n" +
                " combination then it would be difficult to guess the initial seed" + "\r\n";


        }
    }
}