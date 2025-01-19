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
    string[] story = { "여러분들 안녕하세요 장박사입니다.\n인사하세요 이 친구는 테디라는 친구입니다.\n곰인형이지만 제 실험으로 생명을 가지게된 인형입니다.", "생명의 비약을 통해 생명이 없는 곰인형이나 무 생물체를 살아있는 생명체로 변화를 줍니다.", "음.... 신기하지만 생명의 윤리 문제나 법적으로 문제가 있어보이는 결과물이네요 박사님", "저희는 계약하지 않겠습니다. 저희도 안합니다!\n우우..(비난)", "음........ 몇 년간 준비한 발표회인데.... \n힘들게 개발한 제품인데 인정받지못하다니.......", "스윽... 히히", "솨라락..솨라락 킥킥 다들 수고하라고", "끄에엑... 와아아악!! 캬캭..", "도망쳐!! 저것들은 뭐야!!? 쓰레기들이 살아움직인다!! \n 으악!! 살려줘!!", "속보입니다. 장박사가 연구한 물품으로 쓰레기 괴물을 만들고 도주한 것으로 추정됩니다. 현재 조사중에...","두리야 소화 시킬겸 재활용 쓰레기좀 버리고올래? 재활용 잘 할수있지?\n넵!! 재활용이 제 주특기 인걸요", "응...? 저게 뭐지 곰인형인가? 움직이는거같은데 ", "친구야 나 좀 도와줘 세상에 쓰레기들이 갑자기 살아나서 세상을 위협하고있어..", "내 이름은 테디야 친구가 쓰레기를 잡아서 처지해줘  부탁할게...\n그래 알겠어..내이름은 두리야 \n얼른 가보자", "나쁜 쓰레기들은 저기있어 빨리가보자!!" };
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
