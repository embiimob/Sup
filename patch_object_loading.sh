#!/bin/bash
cat << 'DIFF' > patch.diff
<<<<<<< SEARCH
                                    else
                                    {
                                        // Load the original image from file
                                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(imgurn);

                                        // Check if the original image is a GIF
                                        if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                                        {
=======
                                    else
                                    {
                                        if (IsUnsupportedImageFormat(imgurn))
                                        {
                                            foundObject.ObjectImage.ImageLocation = @"includes\HugPuddle.jpg";
                                        }
                                        else
                                        {
                                        // Load the original image from file
                                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(imgurn);

                                        // Check if the original image is a GIF
                                        if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                                        {
>>>>>>> REPLACE
<<<<<<< SEARCH
                                            // Save the resized image as a thumbnail
                                            resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                        }
                                    }
                                }));
                            }
=======
                                            // Save the resized image as a thumbnail
                                            resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                        }
                                        }
                                    }
                                }));
                            }
>>>>>>> REPLACE
DIFF
patch -p1 < patch.diff
