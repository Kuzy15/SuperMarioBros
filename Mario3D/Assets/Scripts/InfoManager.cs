using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    public GameObject[] images;
    public GameObject creditsButton;
    public GameObject controlsButton;
    public GameObject usageButton;
    public GameObject nextButton;
    public GameObject ngramsButton;
    public GameObject rnnButton;
    public GameObject trainButton;
    public GameObject trainLoadButton;
    public GameObject loadButton;
    public GameObject infotitle;
    public GameObject questionButton;
    public GameObject lookingModePanel;

    public GameObject[] ngramsImagesTrain;
    public GameObject[] rnnImagesTrain;

    public GameObject[] ngramsImagesLoadTrain;
    public GameObject[] rnnImagesLoadTrain;

    public GameObject[] ngramsImagesLoad;
    public GameObject[] rnnImagesLoad;


    private bool _canClosePanel = false;
    private bool _ngramsMode = true;
    private bool _selectButtonsOn = false;
    private GameObject _currentImage;
    private int _usageIndex;
    private GameObject[] _imagesToUse;
    private bool _panelShown = false;
    // Start is called before the first frame update
    void Start()
    {
        _usageIndex = 2;
        nextButton.SetActive(false);
        _imagesToUse = images;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnClickReturn()
    {
        if (!_canClosePanel)
        {
            GameManager.GM.ChangeScene(GameManager.SceneFlow.MENU);
        }else if(trainLoadButton.activeSelf || trainButton.activeSelf || loadButton.activeSelf)
        {
            trainButton.SetActive(false);
            trainLoadButton.SetActive(false);
            loadButton.SetActive(false);
            rnnButton.SetActive(true);
            ngramsButton.SetActive(true);
            _usageIndex = 2;
            _ngramsMode = true;
            //_canClosePanel = false;
            _selectButtonsOn = false;
            questionButton.SetActive(false);
        }
        else if (_selectButtonsOn)
        {
            _currentImage.SetActive(false);
            trainButton.SetActive(true);
            trainLoadButton.SetActive(true);
            loadButton.SetActive(true);
            rnnButton.SetActive(false);
            ngramsButton.SetActive(false);
            _usageIndex = 2;
            _ngramsMode = true;
            //_canClosePanel = false;
            _selectButtonsOn = false;
            questionButton.SetActive(false);
        }
        else
        {
            _currentImage.SetActive(false);
            _canClosePanel = false;
            ActiveButtons();
            nextButton.SetActive(false);
            rnnButton.SetActive(false);
            ngramsButton.SetActive(false);
            _usageIndex = 2;
            _imagesToUse = images;
            _ngramsMode = true;
            trainButton.SetActive(false);
            trainLoadButton.SetActive(false);
            loadButton.SetActive(false);
            _selectButtonsOn = false;
            infotitle.SetActive(true);
            questionButton.SetActive(false);
        }
    }

    public void OnClickCredits()
    {
        ActiveImage(0);
    }

    public void OnClickUsage()
    {
        ActiveImage(2);
        nextButton.SetActive(true);
    }

    public void ActiveImage(int image)
    {
        if (image + 1 <= images.Length)
        {
            _currentImage = images[image];
            _currentImage.SetActive(true);
            _canClosePanel = true;
            DeactiveButtons();
            infotitle.SetActive(false);
        }

    }

    public void DeactiveImage(int image)
    {
        if (image + 1 >= images.Length)
        {
            _currentImage = images[image];
            _currentImage.SetActive(false);
            _canClosePanel = false;
        }
    }

    public void DeactiveButtons()
    {
        if (creditsButton.activeSelf)
            creditsButton.SetActive(false);
        if (usageButton.activeSelf)
            usageButton.SetActive(false);
        if (controlsButton.activeSelf)
            controlsButton.SetActive(false);
    }

    public void ActiveButtons()
    {
        if (!creditsButton.activeSelf)
            creditsButton.SetActive(true);
        if (!usageButton.activeSelf)
            usageButton.SetActive(true);
        if (!controlsButton.activeSelf)
            controlsButton.SetActive(true);
    }

    public void OnClickNext()
    {
        if (_imagesToUse == images)
        {
            if (_usageIndex + 1 < _imagesToUse.Length)
            {
                _usageIndex += 1;
                _currentImage.SetActive(false);
                _currentImage = _imagesToUse[_usageIndex];
                _currentImage.SetActive(true);
                Debug.Log(_currentImage);
            }
            else if (_usageIndex == 5)
            {
                Debug.Log("HEY");
                _currentImage.SetActive(false);
                ngramsButton.SetActive(true);
                rnnButton.SetActive(true);
                nextButton.SetActive(false);
            }
            else
            {
                _currentImage.SetActive(false);
                nextButton.SetActive(false);
                ActiveButtons();
                _usageIndex = 2;
                _canClosePanel = false;
            }
        }
        else
        {
            if (_ngramsMode)
            {
                if (_usageIndex + 1 < _imagesToUse.Length)
                {
                    _usageIndex += 1;
                    _currentImage.SetActive(false);
                    _currentImage = _imagesToUse[_usageIndex];
                    _currentImage.SetActive(true);
                    Debug.Log(_currentImage);
                }
                else
                {
                    _currentImage.SetActive(false);
                    nextButton.SetActive(false);
                    //ActiveButtons();
                    _usageIndex = 2;
                    //_canClosePanel = false;
                    trainButton.SetActive(true);
                    trainLoadButton.SetActive(true);
                    loadButton.SetActive(true);
                    //_imagesToUse = images;
                }
            }
            else
            {
                if (_usageIndex + 1 < _imagesToUse.Length)
                {
                    _usageIndex += 1;
                    _currentImage.SetActive(false);
                    _currentImage = _imagesToUse[_usageIndex];
                    _currentImage.SetActive(true);
                    Debug.Log(_currentImage);
                }
                else
                {
                    _currentImage.SetActive(false);
                    nextButton.SetActive(false);
                    //ActiveButtons();
                    _usageIndex = 2;
                    //_canClosePanel = false;
                    trainButton.SetActive(true);
                    trainLoadButton.SetActive(true);
                    loadButton.SetActive(true);
                    //_imagesToUse = images;
                }
            }
        }
    }

    public void OnClickNGrams()
    {
        ngramsButton.SetActive(false);
        rnnButton.SetActive(false);
        trainButton.SetActive(true);
        trainLoadButton.SetActive(true);
        loadButton.SetActive(true);
        _ngramsMode = true;
    }

    public void OnClickRNN()
    {
        ngramsButton.SetActive(false);
        rnnButton.SetActive(false);
        trainButton.SetActive(true);
        trainLoadButton.SetActive(true);
        loadButton.SetActive(true);
        _ngramsMode = false;
    }

    public void OnClickTrain()
    {
        if (_ngramsMode)
        {
            _imagesToUse = ngramsImagesTrain;
        }
        else
        {
            _imagesToUse = rnnImagesTrain;
        }
        _usageIndex = 0;
        _currentImage = _imagesToUse[_usageIndex];
        _currentImage.SetActive(true);
        nextButton.SetActive(true);
        trainButton.SetActive(false);
        trainLoadButton.SetActive(false);
        loadButton.SetActive(false);
        _selectButtonsOn = true;
    }
    public void OnClickLoadTrain()
    {
        if (_ngramsMode)
        {
            _imagesToUse = ngramsImagesLoadTrain;
        }
        else
        {
            _imagesToUse = rnnImagesLoadTrain;
        }
        _usageIndex = 0;
        _currentImage = _imagesToUse[_usageIndex];
        _currentImage.SetActive(true);
        nextButton.SetActive(true);
        trainButton.SetActive(false);
        trainLoadButton.SetActive(false);
        loadButton.SetActive(false);
        _selectButtonsOn = true;
    }
    public void OnClickLoad()
    {
        if (_ngramsMode)
        {
            _imagesToUse = ngramsImagesLoad;
        }
        else
        {
            _imagesToUse = rnnImagesLoad;
        }
        _usageIndex = 0;
        _currentImage = _imagesToUse[_usageIndex];
        _currentImage.SetActive(true);
        nextButton.SetActive(true);
        trainButton.SetActive(false);
        trainLoadButton.SetActive(false);
        loadButton.SetActive(false);
        _selectButtonsOn = true;
    }

    public void OnClickControls()
    {
        ActiveImage(1);
        questionButton.SetActive(true);
    }

    public void OnClickQuestion()
    {
        _panelShown = !_panelShown;
        lookingModePanel.SetActive(_panelShown);
    }
}
