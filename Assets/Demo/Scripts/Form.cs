using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class FormDetais
{
    public string name;
    public string emailID;
    public string mobileNumber;
    public string city;
    public string rating;

}

public class Form : MonoBehaviour
{
    public InputField nameField;
    public InputField emailField;
    public InputField mobileField;
    public InputField cityField;
    public InputField ratingField;
    public GameObject loadingScreen;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdQ9S6i1IjWFEEGvKt4C1jx8fv5BEX_xNOf78OBkCmSLoj6MQ/formResponse";

    private FormDetais form;
    public string path;

    public RawImage image;
    WebCamTexture webcamTex = null;

    byte[] webcamPicData;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string deviceName = devices[0].name;
        webcamTex = new WebCamTexture(deviceName);
        image.texture = webcamTex;
        image.material.mainTexture = webcamTex;
        webcamTex.Play();

        form = new FormDetais();
        nameField.onEndEdit.AddListener(delegate { SetName(nameField); });
        emailField.onEndEdit.AddListener(delegate { SetEmail(emailField); });
        mobileField.onEndEdit.AddListener(delegate { SetNumber(mobileField); });
        cityField.onEndEdit.AddListener(delegate { SetCity(cityField); });
        ratingField.onEndEdit.AddListener(delegate { SetRating(ratingField); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetName(InputField name)
    {
        form.name = name.text;
    }

    void SetCity(InputField city)
    {
        form.city = city.text;
    }

    void SetEmail(InputField email)
    {
        form.emailID = email.text;
    }

    void SetNumber(InputField number)
    {
        form.mobileNumber = number.text;
    }

    void SetRating(InputField rating)
    {
        form.rating = rating.text;
    }

    IEnumerator Post(string name, string email, string mobile, string city, string rating)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => isPicTaken == true);
        webcamTex.Stop();

        // Create a Web Form
        WWWForm form1 = new WWWForm();

        form1.AddField("entry.1636821539", name);
        form1.AddField("entry.2063002654", email);
        form1.AddField("entry.228192549", mobile);
        form1.AddField("entry.39510437", city);
        form1.AddField("entry.292816243", rating);
    https://docs.google.com/spreadsheets/d/1vtfqCmboew-_iK1Ejk_VHEPqpJYLi5pGQE4BImHGrKQ/edit#gid=696405161&range=H1
        form1.AddBinaryData("Pic Upload", webcamPicData, name + ".png", "image/png");

        // byte[] rawData = form1.data;
        // WWW www = new WWW(BASE_URL, rawData);
        using (var w = UnityWebRequest.Post(BASE_URL, form1))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.Log(w.error);
            }
            else
            {
                Debug.Log("Finished Uploading Data");
                
            }
        }
        SceneManager.LoadScene("ARScene");
    }

    public void Submit()
    {
        isPicTaken = false;
        StartCoroutine(TakePic());
        StartCoroutine(Post(form.name, form.emailID, form.mobileNumber, form.city, form.rating));
        // string json = JsonUtility.ToJson(form);
        Debug.Log("Form Details = " + form);

    }

    void SaveTextureToFile(Texture2D texture, string filename)
    {
        System.IO.File.WriteAllBytes(filename, texture.EncodeToPNG());
    }

    bool isPicTaken;
    IEnumerator TakePic()
    {

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("webcam found");
        }
        else
        {
            Debug.Log("webcam not found");
        }
        yield return new WaitForEndOfFrame();
        WebCamTexture wt = image.texture as WebCamTexture;
        Texture2D texture = new Texture2D(wt.width, wt.height);
        Debug.Log("pixels = " + wt.GetPixels());
        texture.SetPixels(wt.GetPixels());
        yield return new WaitForEndOfFrame();

        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.Apply();
        yield return new WaitUntil(() => texture != null);
        image.texture = texture;
        path += form.name + ".png";
        SaveTextureToFile(texture, path);
        webcamPicData = texture.EncodeToPNG();
        isPicTaken = true;

    }

}
