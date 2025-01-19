using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opening : MonoBehaviour
{
    Vector3 target = new Vector3(32, 0, -10);
    public GameObject pScene;
    public Transform[] scenes;
    int sceneNum;
    bool isMoving;
    bool isEND;
    public GameObject[] walls;
    string[] story = { "�����е� �ȳ��ϼ��� ��ڻ��Դϴ�.\n�λ��ϼ��� �� ģ���� �׵��� ģ���Դϴ�.\n������������ �� �������� ������ �����Ե� �����Դϴ�.", "������ ����� ���� ������ ���� �������̳� �� ����ü�� ����ִ� ����ü�� ��ȭ�� �ݴϴ�.", "��.... �ű������� ������ ���� ������ �������� ������ �־�̴� ������̳׿� �ڻ��", "����� ������� �ʰڽ��ϴ�. ���� ���մϴ�!\n���..(��)", "��........ �� �Ⱓ �غ��� ��ǥȸ�ε�.... \n����� ������ ��ǰ�ε� �����������ϴٴ�.......", "����... ����", "�ݶ��..�ݶ�� űű �ٵ� �����϶��", "������... �;ƾƾ�!! ļĽ..", "������!! ���͵��� ����!!? ��������� ��ƿ����δ�!! \n ����!! �����!!", "�Ӻ��Դϴ�. ��ڻ簡 ������ ��ǰ���� ������ ������ ����� ������ ������ �����˴ϴ�. ���� �����߿�...","�θ��� ��ȭ ��ų�� ��Ȱ�� �������� ������÷�? ��Ȱ�� �� �Ҽ�����?\n��!! ��Ȱ���� �� ��Ư�� �ΰɿ�", "��...? ���� ���� �������ΰ�? �����̴°Ű����� ", "ģ���� �� �� ������ ���� ��������� ���ڱ� ��Ƴ��� ������ �����ϰ��־�..", "�� �̸��� �׵�� ģ���� �����⸦ ��Ƽ� ó������  ��Ź�Ұ�...\n�׷� �˰ھ�..���̸��� �θ��� \n�� ������", "���� ��������� �����־� ����������!!" };
    // Start is called before the first frame update
    void Start()
    {
        scenes = pScene.GetComponentsInChildren<Transform>();
        sceneNum = 1;
        isEND = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving || isEND)
            return;
        if(sceneNum >= scenes.Length)
        {
            if (!TextBox.textBox.isStart)
            {
                //GetComponent<Camera>().backgroundColor = new Color(0.1921569f, 0.3019608f, 0.4745098f);
                StartCoroutine(MoveTo(new Vector3(128f, -57f, transform.position.z)));
                foreach (var wall in walls)
                {
                    Destroy(wall);
                }
                isEND = true;
                return;
            }
            else
                return;
        }
        if (!TextBox.textBox.isStart)
        {
            StartCoroutine(MoveTo(sceneNum, story[sceneNum-1]));
        }


    }
    IEnumerator MoveTo(int sceneNumber, string text)
    {
        isMoving = true;
        float count = 0;
        while (true)
        {
            
            yield return null;
            count += Time.deltaTime;
            Vector3 velo = Vector3.zero;
            Vector3 pos = new Vector3(scenes[sceneNumber].position.x, scenes[sceneNumber].position.y, scenes[sceneNumber].position.z - 10);
            //transform.position = Vector3.Slerp(transform.position, pos, 0.01f);
            transform.position = Vector3.SmoothDamp(transform.position, pos,ref velo, 0.03f);
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || count>=1f)
            {
                transform.position = pos;
                break;
            }
        } 
        sceneNum++;
        StartCoroutine(TextBox.textTyping(text));
        isMoving = false;
    }

    IEnumerator MoveTo(Vector3 toPos)
    {
        float count = 0;
        Vector3 wasPos = transform.position;
        while (true)
        {
            count += Time.deltaTime;
            Vector3 velo = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, toPos, ref velo, 0.03f);

            if (count >= 1f)
            {
                transform.position = toPos;
                break;
            }
            yield return null;
        }
    }
}
