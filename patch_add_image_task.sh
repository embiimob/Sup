#!/bin/bash
cat << 'DIFF' > patch.diff
<<<<<<< SEARCH
                            if (File.Exists(imagelocation))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = imagelocation;
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

                                }));
                            }
=======
                            if (File.Exists(imagelocation))
                            {
                                if (IsUnsupportedImageFormat(imagelocation))
                                {
                                    this.Invoke((Action)(() =>
                                    {
                                        AddMedia(imagepath, isprivate, addtoTop);
                                    }));
                                    return;
                                }

                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = imagelocation;
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

                                }));
                            }
>>>>>>> REPLACE
DIFF
