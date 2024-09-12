using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;
public static class Days100
{
    public struct RandomScene
    {
        public List<string> _spots;
        public List<Vector3> _pos;

        public RandomScene(int x)
        {
            this._spots = new List<string>();
            this._pos = new List<Vector3>();
        }

        public void add(string s, Vector3 p) {
            _spots.Add(s);
            _pos.Add(p);
        }

        public Vector3 find(string spot)
        {
            for(int i = 0; i < _spots.Count; i++)
            {
                if (_spots[i] == spot)
                {
                    return _pos[i];
                }
            }
            return Vector3.zero;
        }


    }

    
    public struct AnimDesc
    {
        public string animPath;
        public string animText;
    }
    public struct Segment
    {
        public List<SentenceGroup> groups;

        public Segment(int x)
        {
            groups = new List<SentenceGroup>();
        }

        public void run()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                int r = Random.Range(0, groups[i].sentence.Count - 1);
                string str = groups[i].sentence[r].content;
            }
        }
    }

    public struct SentenceGroup
    {
        public List<Sentence> sentence;

        public SentenceGroup(int x)
        {
            sentence = new List<Sentence>();
        }
    }
    public struct Sentence
    {
       
        public string content;
        public List<ActorSettings> actors;
        public List<CameraSetting> cameras;

        public Sentence(string content)
        {
            this.content = content;
            this.actors = new List<ActorSettings>();
            this.cameras = new List<CameraSetting>();
        }

        public void addActor(ActorSettings actor)
        {
            this.actors.Add(actor);
        }

        public void addCamera(CameraSetting camera)
        {
            this.cameras.Add(camera);
        }
    }

    public static void init()
    {
        RandomScene scene0 = new RandomScene(1);
        scene0.add("came_out_pos0", Vector3.zero);
        scene0.add("came_out_pos1", Vector3.zero);

        List<GameObject> actors = new List<GameObject>();
        int enemy_index = 0;
        Sentence i_came_out_0 = new Sentence("enemy came out from nowhere");
        ActorSettings actor = new ActorSettings(0, 0, actors[enemy_index], MinecraftFighter.Animation.Wait, scene0.find("came_out_pos1"), Quaternion.identity, true);
        i_came_out_0.addActor(actor);

        Sentence i_came_out_1 = new Sentence("enemy came out and charge to me");

        SentenceGroup i_came_out = new SentenceGroup(0);
        i_came_out.sentence.Add(i_came_out_0);
        i_came_out.sentence.Add(i_came_out_1);

        Segment segment = new Segment();
        segment.groups.Add(i_came_out);
    }
}
