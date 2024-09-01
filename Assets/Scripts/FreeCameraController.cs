using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;           // �ƶ��ٶ�
    public float lookSpeed = 2f;            // ��ת�ٶ�
    public float zoomSpeed = 10f;           // �����ٶ�
    public float verticalSpeed = 10f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        // ����Ҽ�������ת
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        // ����WASD����ƽ��
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, 0, moveZ));

        // �����ֿ�������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);

        // CTRL�������������Y���½�
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
        }
    }
}
