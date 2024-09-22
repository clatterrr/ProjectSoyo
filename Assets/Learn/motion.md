# 文本到运动

TMR: Text-to-Motion Retrieval Using Contrastive 3D Human Motion Synthesis

章节一比较有趣

Text-to-Motion Retrieval: Towards Joint Understanding of Human Motion Data and Natural Language

双向编码，训练不了，放弃吧

MotionDiffuse: Text-Driven Human Motion Generation with
Diffusion Model

身体部分独立的控制。由于缺乏文本描述的多样性，我们不能仅从文本描述中实现对身体各部位的精确运动控制。例如，提示“一个人在跑步并挥舞左手”对模型来说具有很大的挑战性，因为预期的运动序列明显远离训练分布。即使我们手动将原始的描述分成两个独立的描述：“一个人在跑步”寻找下肢，“一个人在挥舞左手”寻找上肢，模型仍然很难产生正确的动作。解决这种情况的一个直观方案是分别生成两个运动序列，并将第一个序列的上肢运动和第二个序列的下肢运动结合起来。这个简单的解决方案在某种程度上减轻了这个问题。然而，它忽略了这两部分之间的相关性。特别是“跑步和挥舞左手”，这两个动作的频率应该匹配。否则，由这种幼稚的方法产生的运动就显得不自然了。为了更好地解决这一问题，我们提出了一种身体部分独立的控制方案。

Semantic Parsing for Text to 3D Scene Generation

文本到场景生成的大多数技术挑战源于将语言映射到视觉场景的形式表示的困难，以及当前自然语言处理系统中整体缺乏真实世界的空间知识。这些问题的部分原因是在自然语言中遗漏了关于世界的许多事实。当人们在文本中描述场景时，他们通常只指定重要的相关信息。许多常识事实都没有说明（例如，椅子和桌子通常放在地板上）

Contact Aware Human Motion Generation

主角 动作 身体部位 宾语 

Text-driven Video Prediction

A Cross-Dataset Study for Text-based 3D Human Motion Retrieval

MotionGPT: Human Motion as a Foreign Language



Object Motion Guided Human Motion Synthesis 很炫酷，没啥用

 Generating animated videos of human activities from natural language descriptions 没用 

运动检索

Fast Local and Global Similarity Searches in Large Motion

动作相似度

A Semantic Feature for Human Motion Retrieval

将大量关键字浓缩为5个关键帧，然后比较



# 运动分类

Discriminative Subsequence Mining for Action Classification

没啥意思



随机场景生成

Generating and Ranking Diverse Multi-Character Interactions

第三部分比较有意思，每次都要记录所有物体的动作。也就是动作要根据上下左右自动计算出来

StoryGAN: A Sequential Conditional GAN for Story Visualization



骨骼蒙皮

Rig-Space Physics



想法：脚和手，身体做不同的运动



自然运动IK

Task-based Locomotion

Generating Continual Human Motion in Diverse 3D Scenes 有点的东西



动作识别

Action recognition based on a bag of 3D points

Articulated pose estimation with flexible mixtures-of-parts

Human Action Recognition by Semi-Latent Topic Models

没啥用



数据集

The KIT Motion-Language Dataset

BEHAVE Dataset and Method for Tracking Human Object Interactions

文本到视频

MAKE-A-VIDEO TEXT-TO-VIDEO GENERATION WITHOUT TEXT-VIDEO DATA





https://youtu.be/F4_wbj2Sw2Y?t=217

看看做这个动作会受到动力还是阻力

# 文本总结

Video Summarization with Long Short-term

对于视频摘要，视频帧之间的相互依赖性是复杂的和高度不均匀的。这并不完全令人惊讶，因为人类观众依赖于对视频内容的高级语义理解（并跟踪故事情节的展开）来决定一个框架是否有价值。例如，在决定关键帧是什么时，时间上接近的视频帧通常在视觉上是相似的，因此传递了冗余的信息，因此它们应该被压缩。然而，反之亦然，事实并非如此。也就是说，视觉上相似的帧不需要在时间上很接近。例如，考虑总结视频“早上离开家，回家吃午饭，再离开，晚上回家。”虽然与“在家”场景相关的帧可能在视觉上很相似，但视频的语义流要求它们都不应该被消除。因此，一种只依赖于检查视觉线索而不考虑在长时间跨度内对视频的高级语义理解的摘要算法将错误地消除重要的帧。本质上，是制造t的本质

An interactive video content-based retrieval system

Diverse Sequential Subset Selection for Supervised Video Summarization

Related Work 比较有意思

Large-Scale Video Summarization Using Web-Image Priors

# 镜头语言

Schematic Storyboarding for Video Visualization and Editing

Gesture and speech in interaction: An overview 很有用

# 文本到图像

StyleCLIP: Text-Driven Manipulation of StyleGAN Imagery

从复杂的，但具体的（例如，“特朗普”），不那么复杂和不那么具体的（例如，“莫霍克人”），到更简单和更常见的（例如，“没有皱纹”）。复杂的“特朗普”操纵涉及到几个属性，比如金发、眯着眼睛、张开嘴巴、有点肿胀的脸和特朗普的身份。而一个全局的潜在方向能够捕获主要的视觉属性，而这些属性却不是
