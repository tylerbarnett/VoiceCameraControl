// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace VoiceCameraControl
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton commandBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel listeningLabel { get; set; }

        [Action ("CommandBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CommandBtn_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (commandBtn != null) {
                commandBtn.Dispose ();
                commandBtn = null;
            }

            if (listeningLabel != null) {
                listeningLabel.Dispose ();
                listeningLabel = null;
            }
        }
    }
}