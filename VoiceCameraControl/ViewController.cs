using AVFoundation;
using Foundation;
using Speech;
using System;
using UIKit;
using LibVLCSharp.Shared;
using LibVLCSharp.Platforms.iOS;
using System.Net.Http;

namespace VoiceCameraControl
{
  public partial class ViewController : UIViewController
  {
    const string VIDEO_URL = "rtsp://root:pass@10.1.10.234:554/axis-media/media.amp";

    HttpClient client = new HttpClient();
    private SFSpeechAudioBufferRecognitionRequest recognitionRequest;
    private SFSpeechRecognitionTask recognitionTask;
    private AVAudioEngine audioEngine = new AVAudioEngine();
    private SFSpeechRecognizer speechRecognizer = new SFSpeechRecognizer(new NSLocale("en_US"));
    UIButton buttonIsEnabled = new UIButton();
    VideoView _videoView;
    LibVLC _libVLC;
    LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
    //UILabel listeningLabel = new UILabel();

    public ViewController(IntPtr handle) : base(handle)
    {
    }

    public override void ViewDidLoad()
    {
      base.ViewDidLoad();
      // Perform any additional setup after loading the view, typically from a nib.

      commandBtn.Enabled = false;

      SFSpeechRecognizer.RequestAuthorization((SFSpeechRecognizerAuthorizationStatus auth) =>
      {
        bool buttonIsEnabled = false;
        switch (auth)
        {
          case SFSpeechRecognizerAuthorizationStatus.Authorized:
            buttonIsEnabled = true;
            var node = audioEngine.InputNode;
            var recordingFormat = node.GetBusOutputFormat(0);
            node.InstallTapOnBus(0, 1024, recordingFormat, (AVAudioPcmBuffer buffer, AVAudioTime when) =>
            {
              recognitionRequest.Append(buffer);
            });
            break;
          case SFSpeechRecognizerAuthorizationStatus.Denied:
            buttonIsEnabled = false;
            break;
          case SFSpeechRecognizerAuthorizationStatus.Restricted:
            buttonIsEnabled = false;
            break;
          case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
            buttonIsEnabled = false;
            break;
        }

        InvokeOnMainThread(() => { commandBtn.Enabled = buttonIsEnabled; });
      });
      _libVLC = new LibVLC();
      _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);

      _videoView = new VideoView { MediaPlayer = _mediaPlayer };

      _videoView.Frame = new CoreGraphics.CGRect(0, 0, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height / 2);
      View.AddSubview(_videoView);
      _videoView.MediaPlayer.Play(new Media(_libVLC, VIDEO_URL, FromType.FromLocation));

    }
    public async void CameraCommand(string command)
    {

      var uri = new Uri(string.Format("http://root:pass@10.1.10.234/axis-cgi/com/ptz.cgi?move=left"));
      try
      {
        var response = await client.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
          Console.WriteLine("WORKED");
        }
      }
      catch
      {
        Console.WriteLine("DIDNT WORK");
      }
    }
    public override void DidReceiveMemoryWarning()
    {
      base.DidReceiveMemoryWarning();
      // Release any cached data, images, etc that aren't in use.
    }

    partial void CommandBtn_TouchUpInside(UIButton sender)
    {
      //CameraCommand("command");
      if (audioEngine.Running == true)
      {
        StopSpeechRecognition();
        buttonIsEnabled.SetTitle("Start", UIControlState.Normal);
      }
      else
      {
        StartSpeechRecognition();
        buttonIsEnabled.SetTitle("Stop", UIControlState.Normal);
      }
    }
    public void StartSpeechRecognition()
    {

      listeningLabel.Text = "Listening";
      recognitionRequest = new SFSpeechAudioBufferRecognitionRequest();

      audioEngine.Prepare();
      NSError error;
      audioEngine.StartAndReturnError(out error);
      if (error != null)
      {
        Console.WriteLine(error.ToString());
        return;
      }
      recognitionTask = speechRecognizer.GetRecognitionTask(recognitionRequest, (SFSpeechRecognitionResult result, NSError err) =>
      {
        if (err != null)
        {
          Console.WriteLine(err.ToString());
        }
        else
        {
          if (result.Final == true)
          {
            var results = result.BestTranscription.FormattedString;
            if (results.ToLower() == "left")
            {
              CameraCommand(results);
            }
            listeningLabel.Text = result.BestTranscription.FormattedString;
          }
        }
      });
    }
    public void StopSpeechRecognition()
    {
      audioEngine.Stop();
      recognitionRequest.EndAudio();
    }

  }
}