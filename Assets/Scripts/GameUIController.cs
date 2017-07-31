using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIController : MonoBehaviour {

	public Text WaveText;
	public Text NextWaveText;
	public Text MeteorsIncomingText;
	public Text PowerLabel;
	public Text WavesSurvivedLabel;
	public Text LifeSupportText;

	public Image PowerImageParent;
	public Image PowerImage;

	public Image LifeSupportParent;
	public Image LifeSupprt;

	public Image WelderPowerImage_1;
	public Image WelderPowerImage_2;

    public Image WrenchPowerImage_1;
    public Image WrenchPowerImage_2;

    public Image WelderTool;
	public Image WrenchTool;

	public MeteoriteShower Shower;
	public PowerManager PowerManager;
	public PlayerInventory Inventory;

	public GameObject GameOverUIObject;
	public GameObject GameUIObject;

	public Text[] GameOverTextObjects;

	public int MaxPowerImageWidth;

	private GameController Controller;

	private bool Incoming;
	private bool GameOverCalled = false;
	private int Wave;

	public Sprite Power0;
	public Sprite Power1;
	public Sprite Power2;
	public Sprite Power3;
	public Sprite Power4;

	void Awake ( ) {
		Controller = GetComponent<GameController> ( );
		Incoming = true;
		Wave = -1;
	}

	void Update ( ) {
        if (Inventory.HasWelder) {
            WelderTool.gameObject.SetActive(true);




            if (Inventory.WelderTool.UsesLeft == 8) {
                WelderPowerImage_1.sprite = Power4;
                WelderPowerImage_2.sprite = Power4;
            } else if (Inventory.WelderTool.UsesLeft == 7) {
                WelderPowerImage_1.sprite = Power4;
                WelderPowerImage_2.sprite = Power3;
            } else if (Inventory.WelderTool.UsesLeft == 6) {
                WelderPowerImage_1.sprite = Power4;
                WelderPowerImage_2.sprite = Power2;
            } else if ( Inventory.WelderTool.UsesLeft == 5 ) {
                WelderPowerImage_1.sprite = Power4;
                WelderPowerImage_2.sprite = Power1;
            } else if (Inventory.WelderTool.UsesLeft == 4) {
                WelderPowerImage_1.sprite = Power4;
                WelderPowerImage_2.sprite = Power0;
            } else if (Inventory.WelderTool.UsesLeft == 3) {
                WelderPowerImage_1.sprite = Power3;
                WelderPowerImage_2.sprite = Power0;
            } else if (Inventory.WelderTool.UsesLeft == 2) {
                WelderPowerImage_1.sprite = Power2;
                WelderPowerImage_2.sprite = Power0;
            } else if (Inventory.WelderTool.UsesLeft == 1) {
                WelderPowerImage_1.sprite = Power1;
                WelderPowerImage_2.sprite = Power0;
            } else if (Inventory.WelderTool.UsesLeft <= 0) {
                WelderPowerImage_2.sprite = Power0;
                WelderPowerImage_1.sprite = Power0;
                WelderTool.gameObject.SetActive(false);
            }




		} else {
            WelderPowerImage_2.sprite = Power0;
            WelderPowerImage_1.sprite = Power0;
            WelderTool.gameObject.SetActive ( false );
		}

		if ( Inventory.HasWrench ) {
			WrenchTool.gameObject.SetActive ( true );




            if (Inventory.WrenchTool.UsesLeft == 8)
            {
                WrenchPowerImage_1.sprite = Power4;
                WrenchPowerImage_2.sprite = Power4;
            }
            else if (Inventory.WrenchTool.UsesLeft == 7)
            {
                WrenchPowerImage_1.sprite = Power4;
                WrenchPowerImage_2.sprite = Power3;
            }
            else if (Inventory.WrenchTool.UsesLeft == 6)
            {
                WrenchPowerImage_1.sprite = Power4;
                WrenchPowerImage_2.sprite = Power2;
            }
            else if (Inventory.WrenchTool.UsesLeft == 5)
            {
                WrenchPowerImage_1.sprite = Power4;
                WrenchPowerImage_2.sprite = Power1;
            }
            else if (Inventory.WrenchTool.UsesLeft == 4)
            {
                WrenchPowerImage_1.sprite = Power4;
                WrenchPowerImage_2.sprite = Power0;
            }
            else if (Inventory.WrenchTool.UsesLeft == 3)
            {
                WrenchPowerImage_1.sprite = Power3;
                WrenchPowerImage_2.sprite = Power0;
            }
            else if (Inventory.WrenchTool.UsesLeft == 2)
            {
                WrenchPowerImage_1.sprite = Power2;
                WrenchPowerImage_2.sprite = Power0;
            }
            else if (Inventory.WrenchTool.UsesLeft == 1)
            {
                WrenchPowerImage_1.sprite = Power1;
                WrenchPowerImage_2.sprite = Power0;
            }
            else if (Inventory.WrenchTool.UsesLeft <= 0)
            {
                WrenchPowerImage_1.sprite = Power0;
                WrenchPowerImage_2.sprite = Power0;
                WrenchTool.gameObject.SetActive(false);
            }





        } else {
            WrenchPowerImage_1.sprite = Power0;
            WrenchPowerImage_2.sprite = Power0;
			WrenchTool.gameObject.SetActive ( false );
        }





		if ( Wave != Controller.CurrentWave ) {
			Wave = Controller.CurrentWave;
			WaveText.text = "Wave: " + Wave;
		}

		if ( Controller.WaveEnded ) {
			Controller.WaveEnded = false;
			Incoming = true;
			StartCoroutine ( ShowWaveText ( 1.0f ) );
		}

		if ( !Shower.ShouldSpawn && Incoming ) {
			int time = ( int ) ( Mathf.Abs ( Time.time - Controller.NextWaveStartTime ) );
			NextWaveText.text = "Next Wave in " + time + " seconds";

			if ( time <= 0 ) {
				StartCoroutine ( HideWaveText ( 1.0f ) );
				StartCoroutine ( FlashText ( 3 ) );
				Incoming = false;
			}
		}

		if ( PowerManager.current_station_charge <= 0.0f ) {
			Controller.LifeSupportState -= 0.05f * Time.deltaTime;
		} else {
			Controller.LifeSupportState += 0.025f * Time.deltaTime;
		}

		Controller.LifeSupportState = Mathf.Clamp01 ( Controller.LifeSupportState );

		PowerImage.rectTransform.sizeDelta = new Vector2 ( MaxPowerImageWidth * PowerManager.current_station_charge, PowerImage.rectTransform.sizeDelta.y );
		LifeSupprt.rectTransform.sizeDelta = new Vector2 ( MaxPowerImageWidth * Controller.LifeSupportState, LifeSupprt.rectTransform.sizeDelta.y );

		if ( Controller.LifeSupportState <= 0.0f ) {
			Controller.GameOver = true;
		}

		if ( Controller.GameOver && !GameOverCalled ) {
			StartCoroutine ( HandleGameOver ( ) );
		}
	}

	IEnumerator HandleGameOver () {
		GameOverCalled = true;

		StartCoroutine ( FadeText ( 1.0f, WaveText ) );
		StartCoroutine ( FadeText ( 1.0f, NextWaveText ) );
		StartCoroutine ( FadeText ( 1.0f, PowerLabel ) );
		StartCoroutine ( FadeText ( 1.0f, LifeSupportText ) );
		StartCoroutine ( FadeImage ( 1.0f, PowerImage ) );
		StartCoroutine ( FadeImage ( 1.0f, PowerImageParent ) );
		StartCoroutine ( FadeImage ( 1.0f, LifeSupprt ) );
		StartCoroutine ( FadeImage ( 1.0f, LifeSupportParent ) );
		yield return new WaitForSeconds ( 1.0f );

		GameUIObject.SetActive ( false );
		GameOverUIObject.SetActive ( true );

		foreach ( Text t in GameOverTextObjects ) {
			StartCoroutine ( FadeTextIn ( 1.0f, t ) );
		}

		WavesSurvivedLabel.text = "Waves Survived: " + Controller.CurrentWave;
		WavesSurvivedLabel.gameObject.SetActive ( false );
		WavesSurvivedLabel.gameObject.SetActive ( true );
	}

	IEnumerator FadeTextIn ( float p_time, Text p_text ) {
		float t = 0.0f;
		p_text.color = new Color ( p_text.color.r, p_text.color.g, p_text.color.b, 0.0f );
		while ( t < p_time ) {
			t += Time.deltaTime;
			p_text.color = new Color ( p_text.color.r, p_text.color.g, p_text.color.b, p_text.color.a + Time.deltaTime );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		p_text.color = new Color ( p_text.color.r, p_text.color.g, p_text.color.b, 1.0f );
	}

	IEnumerator FadeImage ( float p_time, Image p_image ) {
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			p_image.color = new Color ( p_image.color.r, p_image.color.g, p_image.color.b, p_image.color.a - Time.deltaTime );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		p_image.color = new Color ( p_image.color.r, p_image.color.g, p_image.color.b, 0.0f );
	}

	IEnumerator FadeText ( float p_time, Text p_text ) {
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			p_text.color = new Color ( p_text.color.r, p_text.color.g, p_text.color.b, p_text.color.a - Time.deltaTime );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		p_text.color = new Color ( p_text.color.r, p_text.color.g, p_text.color.b, 0.0f );
	}

	IEnumerator HideWaveText( float p_time ) {
		yield return new WaitForSeconds ( 2.5f );
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			NextWaveText.color = new Color ( NextWaveText.color.r, NextWaveText.color.g, NextWaveText.color.b, NextWaveText.color.a - Time.deltaTime );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		NextWaveText.color = new Color ( NextWaveText.color.r, NextWaveText.color.g, NextWaveText.color.b, 0.0f );
	}

	IEnumerator ShowWaveText ( float p_time ) {
		float t = 0.0f;
		while ( t < p_time ) {
			t += Time.deltaTime;
			NextWaveText.color = new Color ( NextWaveText.color.r, NextWaveText.color.g, NextWaveText.color.b, NextWaveText.color.a + Time.deltaTime );
			yield return new WaitForSeconds ( Time.deltaTime );
		}
		NextWaveText.color = new Color ( NextWaveText.color.r, NextWaveText.color.g, NextWaveText.color.b, 1.0f );
	}

	IEnumerator FlashText ( int p_numFlashes ) {
		float t = 0.0f;
		float time = 0.5f; // 1 second per flash

		MeteorsIncomingText.gameObject.SetActive ( true );

		for ( int i = 0; i < p_numFlashes; i++ ) {
			t = 0.0f;
			MeteorsIncomingText.color = new Color ( MeteorsIncomingText.color.r, MeteorsIncomingText.color.g, MeteorsIncomingText.color.b, 0.0f );

			while ( t < time ) {
				t += Time.deltaTime;
				MeteorsIncomingText.color = new Color ( MeteorsIncomingText.color.r, MeteorsIncomingText.color.g, MeteorsIncomingText.color.b, MeteorsIncomingText.color.a + ( Time.deltaTime * 2.0f ) );
				yield return new WaitForSeconds ( Time.deltaTime );
			}

			t = 0.0f;
			MeteorsIncomingText.color = new Color ( MeteorsIncomingText.color.r, MeteorsIncomingText.color.g, MeteorsIncomingText.color.b, 1.0f );

			while ( t < time ) {
				t += Time.deltaTime;
				MeteorsIncomingText.color = new Color ( MeteorsIncomingText.color.r, MeteorsIncomingText.color.g, MeteorsIncomingText.color.b, MeteorsIncomingText.color.a - ( Time.deltaTime * 2.0f ) );
				yield return new WaitForSeconds ( Time.deltaTime );
			}
		}

		MeteorsIncomingText.gameObject.SetActive ( false );
	}
}

