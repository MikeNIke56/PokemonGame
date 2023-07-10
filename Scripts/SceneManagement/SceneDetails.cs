using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{
    [SerializeField]
    List<SceneDetails> connectedScenes;

    [SerializeField]
    AudioClip sceneMusic;
    public bool isLoaded { get; private set; }
    List<SavableEntity> savableEntities;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");
            LoadScene();

            GameController.Instance.SetCurrentScene(this);

            if(sceneMusic != null)
                AudioManager.i.PlayMusic(sceneMusic, fade: true);

            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            var prevScene = GameController.Instance.PrevScene;
            if(prevScene != null)
            { 
                var prevLoadedScenes = prevScene.connectedScenes;
                foreach(var scene in prevLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }

                    if (!connectedScenes.Contains(prevScene))
                        prevScene.UnloadScene();
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!isLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            isLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavedEnt();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }
    public void UnloadScene()
    {
        if (isLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    List<SavableEntity> GetSavedEnt()
    {
        var curScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == curScene).ToList();
        return savableEntities;
    }

    public AudioClip SceneMusic => sceneMusic;
}
