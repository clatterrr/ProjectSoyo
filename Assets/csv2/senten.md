第零步：去掉所有词性



```python
import json
 
from openai import OpenAI
 
client = OpenAI(
    api_key="sk-P0a8NcuDKAxw2AniyVK9OxJf1kJ6UQHAFONAD334PfWdcqM0", # 在这里将 MOONSHOT_API_KEY 替换为你从 Kimi 开放平台申请的 API Key
    base_url="https://api.moonshot.cn/v1",
)
 
system_prompt = """
I began to search through the forest。列出所有的主语，谓语，宾语，名词，形容词，动词。

json format格式

{
	"主语": ["1","2"],
	"谓语": ["1","2"],
	"宾语": ["1","2"],
	"名词": ["1","2"],
	"形容词": ["1","2"],
	"动词": ["1","2"],
}
"""
 
completion = client.chat.completions.create(
    model="moonshot-v1-8k",
    messages=[
        {"role": "system",
         "content": "你是 Kimi，由 Moonshot AI 提供的人工智能助手，你更擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。同时，你会拒绝一切涉及恐怖主义，种族歧视，黄色暴力等问题的回答。Moonshot AI 为专有名词，不可翻译成其他语言。"},
        {"role": "user", "content": system_prompt}
    ],
    temperature=0.3,
    response_format={"type": "json_object"}, # <-- 使用 response_format 参数指定输出格式为 json_object
)
 
content = json.loads(completion.choices[0].message.content)
 

    
```



## 步骤

基本上都是V, 第二个N需要修改

不好分类的就手动分类，加入总结剧情库，但不加入句法库

python 程序要能自动矫正

对的话，给出对应的理由，给出可信度

可信度在填表时就要写好

错误的话，矫正后自动记录

可以给一些Summary 打上 ActorReplace，表明可更换主语

当发现一个动词的时候，先去所有可能的地方寻找动词，

可能性1

可能性2

可能性3

一个句子中有一个动词和一个名词

这个动词60%是归类为summary 1，40%归类为summary 2

名词80%归类为summary1，20%归类为 summary 3

那么这个句子归类为summary1,2,3的概率是多少？

代码检查项: 是否添加了不存在的词

是敌是友可根据全局动作得知。敌人现在没攻击我，之后攻击我，那么也是判断为敌人

what the I looked up only to see birds lined up along the valley Ledges hunting for food

line up 可以作为修饰语，不加到总结中，但是在合成时加上。不用管词性

如果同时两个词被选中，都要选，然后算概率

ILookAround IFoundEnemy EnemyAttackFood

都挺重要的，不用删，合成的时候，可以合并成一句

合成时的合理性检测：A Tiger line up ? 询问chatgpt 给出意见，让我自己选择

小Summary 还是安装既定格式的吧，大Summary随意衔接，注意全局变量

为了描述人物性格，动词越多越好

仅有形容词不够，还需要有动词

## 步骤

判断句子里有哪些名词，动词，形容词，不包括 is / was

```
From it came the mutated vultures flying in。列出所有的主语，谓语，宾语，名词，形容词，动词。

json format格式

{
	"主语": ["1","2"],
	"谓语": ["1","2"],
	"宾语": ["1","2"],
	"名词": ["1","2"],
	"形容词": ["1","2"],
	"动词": ["1","2"],
}

what the I looked up only to see birds lined up along the valley Ledges hunting for food

拆分为简单句。去掉倒装等非常规结构，使用简单句式结构 ，转换为一般现在时
简单句应为以下格式之一。
1. 名词 + 动作
2. 名词 + 动作 + 名词
3. 名词 + is + 形容词
4. 名词 + 动词 + 名词 + to + 名词
5. 名词 + 任意连接词 + that (从句)
6. 未知格式列出所有的主语，谓语，宾语之间的关系

json format格式。有的话就填空，没有的话就不填就行

{
	"简单句内容" : "..."
	"简单句格式全称" : "..."
	"简单句格式索引" : "..."
	"主语1" : "..."
	"主语1对应的动作1": "...",
	"主语1对应的动作1的宾语": "...",
	"主语1动作1所在地点": "...",
	"主语1" : "..."
	"主语1对应的动作2": "...",
	"主语1对应的动作2的宾语": "...",
	"主语1动作2所在地点": "...",
	"主语2" : "..."
	"主语2对应的动作1": "...",
	"主语2对应的动作1的宾语": "...",
	"主语2对应的动作1的地点": "...",
}
```





```
oh no I began to inspect the farm when I suddenly had an idea 拆分为简单句
简单句应为以下格式之一。格式前面的数字为简单句格式索引
1. 名词 + 动作
2. 名词 + 动作 + 名词
3. 名词 + is + 形容词
4. 名词 + 动词 + 名词 + to + 名词
5. 名词 + 任意连接词 + that (从句)
6. 未知格式

json format格式

{
	"简单句内容": "...",
	"简单句格式": "...",
	"简单句格式索引": "...",
	
	"简单句内容": "...",
	"简单句格式": "...",
	"简单句格式索引": "...",
}
```



### 第五步

拆分为简单句后，补充说明添加很容易，去掉很容易

```json


I jumped into the battle and began to protect the innocent animal while fighting against them
```

i jumped into the battle

i begin to protect the innocent animal

while 不管了



```json
I need to find my mom and dad isolated to the chaos
```

i need to ....

i find my mom

my mom is isolated ( to the chaos )



### **0. 名词 + 实际动作** N V

- i died
- i 

### 1. **名词 + 实际动作 + 名词人物** N V N

- he slash me
- i follow him
- i find my mom

### T. 名词 + 实际动作 + 名词 物品/ 地点 N V N

- i find a place
- i pick up a ore

------

### 2. **名词 + is + 形容词 **N is adj

表明状态

- he is strong
- i was hurt

### 2. **名词 + is + 名词 **N is N

表明状态is / become

- he is strong
- i was hurt

### T. 名词 + 动词 + 名词 + to + 名词 n v n to n

- she give a gift to me
- he slash a power on me

------

### 3. **名词 + want / need / feel + 名词** n vv n

主角需要，但还没有实际发生的

------

### 4. 名词 + want / need / found / see + that + 句子 n that 

复杂句式

### 5. 句子加方位名词 + place



# 细分规则

## 句子调整

### 所有时态改为一般现在时

### 倒装改正常

```json
I was being pursued by the Mythic hydra's Army of elite soldiers
```

soldiers pursue me

### 拆分为多个简单句

省略一切修饰语 (记录)





```json
I made my way back over to the creepers and handed them my prize
```

i walk to creepers

i handed creepers prize