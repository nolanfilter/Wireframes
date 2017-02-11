using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

    public GameObject[] screens;
    private int screenIndex = -1;

    public int screenWidth = 1334;
    public int screenHeight = 750;

    void Start()
    {
        StartCoroutine( "DoGenerate" );
    }

    private IEnumerator DoGenerate()
    {
        if( !Application.isEditor )
        {
            Screen.SetResolution( screenWidth, screenHeight, false );

            Canvas canvas = GetComponent<Canvas>();

            if( canvas )
            {
                canvas.renderMode = RenderMode.WorldSpace;
            }

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.AngleAxis( 90f, Vector3.forward );
            transform.localScale = Vector3.one;

            RectTransform rectTransform = GetComponent<RectTransform>();

            if( rectTransform )
            {
                rectTransform.sizeDelta = new Vector2( screenWidth, screenHeight );
            }

            if( Camera.main )
            {
                Camera.main.orthographicSize = screenHeight / 2;
            }

            yield return null;
        }
        else
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }
            
        for( int i = 0; i < screens.Length; i++ )
        {
            if( screens[ i ] != null )
            {
                screens[ i ].SetActive( false );
            }
        }

        Texture2D texture = new Texture2D( screenWidth, screenHeight );
        Rect rect = new Rect( 0, 0, screenWidth, screenHeight );

        for( int i = 0; i < screens.Length; i++ )
        {
            if( screenIndex >= 0 && screenIndex < screens.Length && screens[ screenIndex ] != null )
            {
                screens[ screenIndex ].SetActive( false );
            }

            screenIndex++;

            if( screenIndex >= 0 && screenIndex < screens.Length && screens[ screenIndex ] != null )
            {
                screens[ screenIndex ].SetActive( true );

                yield return new WaitForEndOfFrame();

                texture.ReadPixels( rect, 0, 0 );
                texture.Apply();

                System.IO.File.WriteAllBytes( System.IO.Path.Combine( System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop ), screens[ screenIndex ].name + ".png" ), texture.EncodeToPNG() );
            }
        }

        Application.Quit();
    }
}
