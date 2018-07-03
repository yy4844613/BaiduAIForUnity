using UnityEngine;
using Baidu.Aip.ImageClassify;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
public class Result
{
  public string name;
  public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string score;
    public string Score
    {
        get { return score; }
        set { score = value; }
    }
}



public class _TestPic : MonoBehaviour {
    public string apikey="";
    public string screatKey = "";
    ImageClassify imc;
    string imagePath = "";
    public string fileName = "";
    public Text showRes;
    string jsonString;



    //public WebCamTexture cameraTexture;
    //public string cameraName = "";
    public Image littlePhoto;


    public RawImage rawimage;
    Texture2D imageScreenShoot;
    public GameObject canvas;


    WebCamTexture camTexture;
 public    CanvasScaler CanScaler;
 public    Camera ca;
  public   Image img;
    //  var client = new Baidu.Aip.ImageClassify.ImageClassify(apiKey,secretKey);
    // Use this for initialization
    void Start () {
        imagePath = Application.streamingAssetsPath+"/"+fileName;
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        imc = new ImageClassify(apikey,screatKey);
        imc.Timeout = 60000;
        //#if UNITY_IOS || UNITY_IPHONE
        //       rawimage.transform.Rotate (new Vector3 (0, 180, 90));
        //#elif UNITY_ANDROID
        //        rawimage.transform.Rotate (new Vector3 (0, 0, 90));
        //#endif
        //       // StartCoroutine(OpenCamera());
        imageScreenShoot = new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);



      //  img = GetComponentInChildren<Image>();

        //CanScaler = GetComponentInChildren<CanvasScaler>();
        CanScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

       // ca = GetComponentInChildren<Camera>();
        ca.orthographicSize = Screen.width / 100.0f / 2.0f;

        img.transform.localScale = new Vector3(-1, -1, -1);

        img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        img.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.height);
        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width);

        // 设备不同的坐标转换
#if UNITY_IOS || UNITY_IPHONE
        img.transform.Rotate (new Vector3 (0, 180, 90));
#elif UNITY_ANDROID
        img.transform.Rotate (new Vector3 (0, 0, 90));
#endif

        StartCoroutine(CallCamera());

















    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Go()
    {
        StartCoroutine(TakeAPhoto());
        _DebugText.instance.De("Go");
    }

     void TestFunc(byte[] imageBytes)
    {
       // _DebugText.instance.De("TestFunc");

        // byte[] bytesPNG = GetImageByte();
        JObject jj = imc.AnimalDetect(imageBytes);
        //   imc.AnimalDetect(GetImageByte());
        // Dictionary<string, string> jsdata = new Dictionary<string, string>();
        

       // Console.WriteLine(jj);
        foreach (var item in jj)
        {
            jsonString = item.Value.ToString();
        
        }
       // _DebugText.instance.De(jsonString+  "JS!!!");

        List<Result> rrr = new List<Result>();
        rrr =JsonConvert.DeserializeObject<List<Result>>(jsonString);

        showRes.text = "识别结果：" + rrr[0].name + "  相似度：" +( Convert.ToSingle(rrr[0].score.ToString())*100).ToString()+"%";
      //  _DebugText.instance.De("识别结果：" + rrr[0].name + "  相似度：" + (Convert.ToSingle(rrr[0].score.ToString()) * 100).ToString() + "%");

    }
   
    public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain,
        // look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }
        return isOk;
    }
    //IEnumerator OpenCamera()
    //{
    //    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
    //    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    //    {
    //        WebCamDevice[] devices = WebCamTexture.devices;
    //        cameraName = devices[0].name;
    //        cameraTexture = new WebCamTexture(cameraName, Screen.width, Screen.height, 15);
    //        rawimage.texture = cameraTexture;
    //        cameraTexture.Play();
    //    }
    //}
    IEnumerator TakeAPhoto()
    {
       // _DebugText.instance.De("TkePhoto");
        Texture2D temp = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
       
        canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        temp.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        temp.Apply();


        littlePhoto.sprite = Sprite.Create(temp, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));
         
        imageScreenShoot.ReadPixels(new Rect(0, 0, Screen.width-400, Screen.height-400), 0, 0, false);
        canvas.SetActive(true);
       
        imageScreenShoot.Apply();
        
       // imageScreenShoot.format=TextureFormat.
        byte[] bs = imageScreenShoot.EncodeToPNG();
        print(bs.Length);
        TestFunc(bs);
    }
    IEnumerator CallCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (camTexture != null)
                camTexture.Stop();

            WebCamDevice[] cameraDevices = WebCamTexture.devices;
            string deviceName = cameraDevices[0].name;

            camTexture = new WebCamTexture(deviceName, Screen.height, Screen.width, 60);
            img.canvasRenderer.SetTexture(camTexture);

            camTexture.Play();
        }
    }







}
