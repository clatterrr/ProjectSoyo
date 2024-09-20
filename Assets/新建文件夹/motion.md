文本到运动

TMR: Text-to-Motion Retrieval Using Contrastive 3D Human Motion Synthesis

章节一比较有趣

Text-to-Motion Retrieval: Towards Joint Understanding of Human Motion Data and Natural Language

双向编码，训练不了，放弃吧

MotionDiffuse: Text-Driven Human Motion Generation with
Diffusion Model

身体部分独立的控制。由于缺乏文本描述的多样性，我们不能仅从文本描述中实现对身体各部位的精确运动控制。例如，提示“一个人在跑步并挥舞左手”对模型来说具有很大的挑战性，因为预期的运动序列明显远离训练分布。即使我们手动将原始的描述分成两个独立的描述：“一个人在跑步”寻找下肢，“一个人在挥舞左手”寻找上肢，模型仍然很难产生正确的动作。解决这种情况的一个直观方案是分别生成两个运动序列，并将第一个序列的上肢运动和第二个序列的下肢运动结合起来。这个简单的解决方案在某种程度上减轻了这个问题。然而，它忽略了这两部分之间的相关性。特别是“跑步和挥舞左手”，这两个动作的频率应该匹配。否则，由这种幼稚的方法产生的运动就显得不自然了。为了更好地解决这一问题，我们提出了一种身体部分独立的控制方案。

Semantic Parsing for Text to 3D Scene Generation

文本到场景生成的大多数技术挑战源于将语言映射到视觉场景的形式表示的困难，以及当前自然语言处理系统中整体缺乏真实世界的空间知识。这些问题的部分原因是在自然语言中遗漏了关于世界的许多事实。当人们在文本中描述场景时，他们通常只指定重要的相关信息。许多常识事实都没有说明（例如，椅子和桌子通常放在地板上）





运动检索

Fast Local and Global Similarity Searches in Large Motion

动作相似度

A Semantic Feature for Human Motion Retrieval

将大量关键字浓缩为5个关键帧，然后比较





随机场景生成

Generating and Ranking Diverse Multi-Character Interactions

第三部分比较有意思，每次都要记录所有物体的动作。也就是动作要根据上下左右自动计算出来



骨骼蒙皮

Rig-Space Physics



想法：脚和手，身体做不同的运动



自然运动IK

Task-based Locomotion



动作识别

Action recognition based on a bag of 3D points

没啥用



数据集

The KIT Motion-Language Dataset



