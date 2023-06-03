using UnityEngine;

public class MonkeEatFood : MonoBehaviour
{
[Header("By TheKachow dont steal or change give cred with yt.")]
    public AudioClip eatingSound;
    public float respawnTime = 10f;
    public float eatInterval = 3f;

    private bool hasBeenEaten = false;
    private float timeSinceLastEaten = 0f;
    private float timeSinceLastMeal = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera") && gameObject.CompareTag("Food") && !hasBeenEaten && timeSinceLastEaten >= respawnTime && timeSinceLastMeal >= eatInterval)
        {
            AudioSource.PlayClipAtPoint(eatingSound, transform.position);
            hasBeenEaten = true;
            gameObject.SetActive(false);

            Invoke(nameof(RespawnFood), respawnTime);
            timeSinceLastEaten = 0f;
            timeSinceLastMeal = 0f;

            // Notify all other food objects in the scene that a meal has been eaten
            var allFoodObjects = FindObjectsOfType<MonkeEatFood>();
            foreach (var foodObject in allFoodObjects)
            {
                if (foodObject != this)
                {
                    foodObject.OnMealEaten();
                }
            }
        }
    }

    private void RespawnFood()
    {
        gameObject.SetActive(true);
        hasBeenEaten = false;
    }

    private void Update()
    {
        timeSinceLastEaten += Time.deltaTime;
        timeSinceLastMeal += Time.deltaTime;
    }

    private void OnMealEaten()
    {
        timeSinceLastMeal = 0f;
    }
}
