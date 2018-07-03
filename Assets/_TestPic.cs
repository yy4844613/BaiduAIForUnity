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


//读取JSON 用的类
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

// 自己申请的KEY
    public string apikey="";
    public string screatKey = "";
    ImageClassify imc;
  
    public Text showRes;
    string jsonString;

    public Image littlePhoto;

    Texture2D imageScreenShoot;
    public GameObject canvas;


    WebCamTexture camTexture;
    public    CanvasScaler CanScaler;
    public    Camera ca;
    public   Image img;
  
    void Start () {
        //这句话解决不能连接到主机的问题
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
       //初始化
        imc = new ImageClassify(apikey,screatKey);
        imc.Timeout = 60000;


        imageScreenShoot = new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);
        CanScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
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

    public void Go()
    {
        StartCoroutine(TakeAPhoto());
       
    }

     void TestFunc(byte[] imageBytes)
    {
       
        JObject jj = imc.AnimalDetect(imageBytes);
        foreach (var item in jj)
        {
            jsonString = item.Value.ToString();
        
        }
        List<Result> rrr = new List<Result>();
        //有6个结果  第一个相似度 最高    这里只取了第一个
        rrr =JsonConvert.DeserializeObject<List<Result>>(jsonString);
        showRes.text = "识别结果：" + rrr[0].name + "  相似度：" +( Convert.ToSingle(rrr[0].score.ToString())*100).ToString()+"%";
    }
   //这个方法 解决了不能  不能连接到主机的问题
    public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
     
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
    //不用解释
    IEnumerator TakeAPhoto()
    {
       
        Texture2D temp = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
       
        canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        temp.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        temp.Apply();


        littlePhoto.sprite = Sprite.Create(temp, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));
         
        imageScreenShoot.ReadPixels(new Rect(0, 0, Screen.width-400, Screen.height-400), 0, 0, false);
        
       
        imageScreenShoot.Apply();
    
        byte[] bs = imageScreenShoot.EncodeToPNG();
       
        canvas.SetActive(true);
        TestFunc(bs);
    }
    //不用解释
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
