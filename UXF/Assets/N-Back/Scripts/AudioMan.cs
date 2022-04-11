using UnityEngine;


public class AudioMan : MonoBehaviour
{
    [SerializeField] private AudioSource s_miss;
    [SerializeField] private AudioSource s_wrong;
    [SerializeField] private AudioSource s_success_hit;
    [SerializeField] private AudioSource s_success_no_hit;


    public void PlayMiss()
    {
        s_miss.Play();
    }


    public void PlayWrong()
    {
        s_wrong.Play();
    }


    public void PlaySuccessHit()
    {
        s_success_hit.Play();
    }


    public void PlaySuccessNoHit()
    {
        s_success_no_hit.Play();
    }
}