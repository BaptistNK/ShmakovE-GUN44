using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    private IInputService _inputService;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
        Debug.Log("[PlayerMovement] Сервис ввода успешно внедрен через Zenject!");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
