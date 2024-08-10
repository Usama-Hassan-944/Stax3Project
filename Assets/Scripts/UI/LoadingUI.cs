using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public List<GameObject> dots;
    private void Awake()
    {
        foreach (var dot in dots)
        {
            dot.gameObject.SetActive(false);
        }
    }
    private async void Update()
    {
        foreach (var dot in dots)
        {
            if (dot != null)
            {
                dot?.gameObject.SetActive(!dot.activeSelf);
                await Delay();
            }
            else
                continue;
        }
    }

    async Task Delay()
    {
        await Task.Delay(500);
    }
}
