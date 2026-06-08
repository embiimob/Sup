#!/bin/bash
cat << 'DIFF' > patch.diff
<<<<<<< SEARCH
                    else
                    {
                        string thumbnailPath = imagelocation + "-thumbnail.jpg";

                        // Check if a thumbnail exists
                        if (System.IO.File.Exists(thumbnailPath))
                        {
                            pictureBox.ImageLocation = thumbnailPath;
                            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                        }
                        else
                        {
=======
                    else
                    {
                        if (IsUnsupportedImageFormat(imagelocation))
                        {
                            msg.Dispose();
                            AddMedia(imagepath, isprivate, addtoTop);
                            return;
                        }

                        string thumbnailPath = imagelocation + "-thumbnail.jpg";

                        // Check if a thumbnail exists
                        if (System.IO.File.Exists(thumbnailPath))
                        {
                            pictureBox.ImageLocation = thumbnailPath;
                            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                        }
                        else
                        {
>>>>>>> REPLACE
DIFF
