#!/bin/bash
cat << 'DIFF' > patch.diff
<<<<<<< SEARCH
                                        else
                                        {
                                            // Load the original image from file
                                            Image originalImage = Image.FromFile(imgurn);

                                            // Check if the original image is a GIF
                                            if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
=======
                                        else
                                        {
                                            // Ensure we do not crash on AVIF/WEBP
                                            if (IsUnsupportedImageFormat(imgurn))
                                            {
                                                foundObject.ObjectImage.ImageLocation = @"includes\HugPuddle.jpg";
                                            }
                                            else
                                            {
                                            // Load the original image from file
                                            Image originalImage = Image.FromFile(imgurn);

                                            // Check if the original image is a GIF
                                            if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
>>>>>>> REPLACE
<<<<<<< SEARCH
                                                // Save the resized image as a thumbnail
                                                resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                            }
                                        }
                                    }));
                                }
                                else
=======
                                                // Save the resized image as a thumbnail
                                                resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                            }
                                            }
                                        }
                                    }));
                                }
                                else
>>>>>>> REPLACE
<<<<<<< SEARCH
                            else
                            {
                                // Load the original image from file
                                Image originalImage = Image.FromFile(imgurn);

                                // Check if the original image is a GIF
                                if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
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
                                Image originalImage = Image.FromFile(imgurn);

                                // Check if the original image is a GIF
                                if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
>>>>>>> REPLACE
<<<<<<< SEARCH
                                    // Save the resized image as a thumbnail
                                    resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                }
                            }

                        }
                        else
=======
                                    // Save the resized image as a thumbnail
                                    resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                }
                                }
                            }

                        }
                        else
>>>>>>> REPLACE
<<<<<<< SEARCH
                                else
                                {
                                    // Load the original image from file
                                    Image originalImage = Image.FromFile(imgurn);

                                    // Check if the original image is a GIF
                                    if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
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
                                    Image originalImage = Image.FromFile(imgurn);

                                    // Check if the original image is a GIF
                                    if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
>>>>>>> REPLACE
<<<<<<< SEARCH
                                        // Save the resized image as a thumbnail
                                        resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                    }
                                }
                            }));
                        }
                        else
=======
                                        // Save the resized image as a thumbnail
                                        resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                    }
                                    }
                                }
                            }));
                        }
                        else
>>>>>>> REPLACE
DIFF
