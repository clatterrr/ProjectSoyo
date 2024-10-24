using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D texture;
    public Shader shader;
    void Start()
    {
        for(int i = 0; i < texture.width; i++)
            for(int j = 0; j < texture.height; j++)
            {
                Debug.Log("i == " + i + " j == " + j + " color " + texture.GetPixel(i, j));
            }
    }

    /*
 https://youtu.be/yqQuSPOua-0?t=21
Days 1  EnemyChaseMe IRunAway IHavePower ICraftTools IamHungry IEatFarms VilagerStopMe VillagerTalkToMe ZombieAttackVillager IkillZombie VillagerSaveMe
 
 
    =============================================== Generate Scripts ===========================================

    https://youtu.be/WXX7g304xNY?t=222

    Intro                   when it comes to creating a script
    ItLooksGood             you can ask chat GPT to do it for you
    ItIsBad                 it often sounds kind of robotic and a robotic boring script will result in a boring video not good
    IUseToolSimple          instead I like to use a website called dup du
    Feature                  it's basically a website that lets you create scripts that sound like they're written by a human 
    Feature                 they're probably using the chat GPT API but it's been trained on YouTube scripts
    ItIsGood                so it'll be a lot better
    IfYouWant               so if you want to go with The Proven concept for the script which often turns out to perform even better
    SearchNiche             head over to YouTube to search for other channels in your Niche
    AnysisVideo             once you find a channel sort their videos by popularity and copy the link of one of their viral videos
    ClickGenerate           then go back to dubdub click on transcription then choose YouTube and paste the link you'll then get the script
    Ready                   this way you'll have a script that's already proven to work
    OpenWebsite             so head over to chat GPT
    TaskRewrite             paste the script and ask it to rewrite it with other words while keeping the same message

    https://youtu.be/X1wVW-ENssU?t=118

    Step                    step one make the prompts for generating images
    MyGoal                  to create these types of videos we need specific prompts for an AI image generating tool
    Explained               that can easily make these animal images to do that
    IUseTool                we'll ask chat GP to give us five detailed prompts that we can use with leonardoe to create our hybrid animals (detailed prompt)
    TypePrompt              I would also ask for an additional prompt that
    Already                 we now have our hybrid animal prompt and an additional prompt for both animals
    NextStep                we're all set to move to the next step

    https://youtu.be/znb3_gFSaac?t=154

    Transition              now that you got your Niche it's time to design the content
    IUseToolSimple          and guess what we're going to let AI do most of the heavy lifting 
    Theory                  work smarter not harder
    OpenWebsite             first things first open up chat gbt
    TypePrompt              and type in a prompt like ...
    WaitForAI               boom chat GPT Just Whipped up a killer outline for you now
    YouNeed                 you need a solid presentation for your course
    IGotYou                 don't worry I got you

    https://youtu.be/znb3_gFSaac?t=181

    OpenWebsite             head over to pop.AI 
    SignIn                  sign up with your email
    IntroPage               and then you see the interface like this
    PastePrompt              now paste that content into the AI presentation generator
    Generate                hit that arrow
    WaitForAI               and in literally minutes you'll have a Sleek professional presentation outline ready to go
    ClickButton              hit the generate presentation button 
    WaitForAI               and Bam pop AI will make a live presentation in seconds 
    Features                this tool lets you make one free presentation daily

    https://youtu.be/misTjofTN64?t=99

    Step                    the first thing we need to do is think of some video ideas
    OpenWebsite              go to chat GPT and
    TypePrompt               ask it for popular film series
    DontWorry               don't stress about perfecting the prompts
    ForYourConvenice        I'll include the exact ones in the Google Doc for you to use
    WaitForAI               as you can see the AI is already providing a list of series
    WeCanDo                 we can use in our video all right

    https://youtu.be/znb3_gFSaac?t=181 ???

    HowCanWeDoSth           how you can achieve anything to generate the script
    WeUseSth                we use chat GPT so just go ahead and open a new chat
    ICreatePromptForScript  I've created perfect prompts that will give you a script
    Describe                as you can see it's pretty long
    ForYourConvience        so I'll give you a Google doc Link in the description so you can get all the prompts that I've used in this video

    Tips_Summary            in this Niche 3 to 4 minutes of video perform well
    Tips_DontDo             so we will not make 10 to 15 minutes long videos
    Tips_Do                 instead make it short under 5 minutes

    https://youtu.be/DRB8NJLAw6E?t=308
    IUseToolSimple          for script I will use chat GPT
    IUseDetailed            I will simply use a special prompt that I made to get the best possible script
    ForYourConvience        you can access the prompt from the description feel free to use it in the prompt
    MyGoal                  I have asked chat GPT to provide me script for whatever movie
    MyGoalDetailed          for instance we will take the movie named the Meg
    YouUseYouLike           but you can use any movie you like
    DoItYourself            but again I would advise you to read the script for once and do some changes in it if needed and make it more engaging
    MethodYouNoNeed         by getting your script through this method you won't have to waste your time in watching the full movie and writing its explanation
    MethodYouJust           just read the script and boom you got all the idea about what is happening in the movie

    https://youtu.be/pVykPqQDIe8?t=307

    GoChat                  then I would come over to our good friend chat GPT
    AskGPT                  and ask it for the benefits of these ingredients
    AnysisResult            since the first ingredient is ashwagandha we can see that ashwagandha really helps enhance Sleep Quality and treats insomnia

    https://youtu.be/3Vzfoq73iYQ?t=188

    UseChatGPT              to do that we will open chat GPT and use this prompt
    Prompt                  I'm creating a 30second Tik Tok video promoting your product I want you to rewrite the following script using exactly the same structure writing style tone and length and here you need to insert the original script from the competitor's video
    GPTResult               so by using this prompt chat GPT will generate a script similar to the one that went viral
    ForExample              for example the original script starts with ... and the one we generated begins with ...
    ExplainDetail            so it follows the same style but isn't a duplicate

    https://youtu.be/pT3ZUZ2DL3g?t=65

    UseChatGPT              and search for chat GPT
    IUseDetailed3           once it opens you should type reword this and paste the script you copied (use chatgpt level0, use prompt level2, type word level3)
    UsageDetail             this will make the script monetizable
    ItIsImportant           that is why it's very important
    PromptToImage           after chat GPT is done type this create prompts from this to use in Leonardo Ai
    Explain                 and chat GPT will create prompts for you to generate images in Leonardo

    https://youtu.be/pT3ZUZ2DL3g?t=157

    PleaseDoSth             now add this at the beginning of your script
    Prompt                  the last one will surprise you
    ItLooksBad              this might look like nothing
    ButTrustMe              but believe me
    ItGood                  a lot of people will watch your video to the end
    Explained               because they want to know what the last

    https://youtu.be/VOfC13e3oHk?t=404

    LookExample             In just take a look at the video I was able to make with this formula
    Go                      so let's Dive Right In
    Step                    the first step is writing a script for this part
    PleaseDoSth             I'll advise you to write it yourself
    Explained               and this is why chat GPT and other AI tools will generate a script that looks Faker than my sister's Gucci bag
    SoYouNeed               so to avoid this you need to learn how to write an engaging script all by yourself
    IUseToolSimple          but once again for the sake of this tutorial I'll use the greatest AI of all time chat GPT plus
    Compare                 it isn't great but it's actually smarter than the old chat GPT
    ScriptWrite             so to generate your script go to type this
    ItLooksBad              this prompt sounds like it was written by a 3-year-old
    ItGood                  but AI can easily handle any prompt you type

    https://youtu.be/nsGvN6ZU-rE?t=42
    
    OpenWebsite             to get started open chat GPT 
    MyGoal                  in the text box you'll need to type a clear request for the image you want
    DetailedExample         for the first image let's say you want to create a scene with a gorilla and an eagle
    TypePrompt              you can type something like create a prompt for ...
    WaitForAI               once you hit enter Chad GPT will will provide you with a detailed image prompt
    ExplainedWhat           it might describe the colors the setting and the positions of the animals (what happened)
    ExplainedEffect         giving you a clear idea of how they look and interact in the scene (is what we want)

    Triky                   using the same background as before
    Explained               this is important because having a consistent background makes the video flow better keeping the viewers engaged
    WaitForAI               Chachi PT will again provide a detailed description for this hybrid creature
    SelectYouWant            you can always modify these prompts if you have specific ideas or features you want to include
    Trans                   once you have both prompts ready you're set to turn these ideas into stunning visuals

    https://youtu.be/kl9jhLlMtyc?t=242

    ItIsGood                okay so short X actually makes the entire script for you
    ButAbstract             but there's some things you need to know
    ItIsBad                  AI can write sure but the scripts you get suck 
    WithBadStrategy         if you don't prompt the right way
    YouMustDo               that's why you have to really put time into your prompt to get a script that's actually entertaining
    Intro                   when it comes to creating a script
    Strategy                I used to think the shorter the better
    Explained               because then the video will get higher retention
    ExplainedMoreDetailed   here's the catch a higher retention doesn't always mean better results
    YoutubeAlgorithms       see YouTube wants people to stay on their platform longer so they tend to promote longer videos
    YoutubeAlgorithms        otherwise people would just post videos that are just a couple of seconds long and get an insane retention which is just not how it works
    Strategy                my point is make your shorts longer
    ItIsGodd                luckily for you when using an AI tool like this longer scripts don't mean more work
    Features                as short X writes the script for you 
    ItIsEasy                all you got to do is type in a good prompt in this box right here
    ItIsBad                  short X is using chat gpt3 so if you want to really spice up your video
    AnotherStrategy         you could use GPT 4 and just copy and paste it into short X


    ========================================= Image ===========================================

    https://youtu.be/bJx7UwrSXcI?t=54

    Step                    our first step is to create hybrid animal pictures
    Noticed                 in the reals you've probably noticed they begin with a still image of two animals side by side
    Noticed                 each labeled with their names and then smoothly transition into a short clip showing the hybrid version
    IWillDo                 that's exactly the format we'll be using to generate prompts\
    IUseToolSimple          we'll need to use chat GPT
    TypePrompt              simply give it this prompt
    Prompt                  I'd like to generate images of hybrid animals can you provide 10 detailed prompts for these hybrids
    WaitForAI               chat GPT provides us with image prompts
    Theory                  and you can really think outside the box don't limit yourself
    ViewersLike             this type of imaginative content is exactly what audiences love especially when it's something that doesn't exis

    https://youtu.be/bJx7UwrSXcI?t=91

    SelectYouWant           for this step you can choose from Leonardo ideogram or mid Journey
    IUseToolSimple          I'll be using Leonardo AI
    SignIn                   to get started sign into Leonardo with your Google account which will give you 150 credits daily
    FirstPage               first navigate to the image generation tab
    PastePrompt             here copy the prompt from chat GPT and paste it into the provided field
    SelectStyle             make sure to focus on the settings when generating separate images of the animal and the superhero select the preset Leonardo Phoenix
    SelectRatio             also set the aspect ratio to 9x6
    ClickGenerate           and then click on generate
    WaitForAI               it will take a few seconds and now the pictures are ready the results
    Good                    look great 
    Download                go ahead and download one of them

    https://youtu.be/nsGvN6ZU-rE?t=110

    IUseToolSimple          next we'll use ideogram
    ToolIntro               a powerful AI tool to turn those prompts into real images this tool is userfriendly and designed to make image creation easy
    PastePrompt             start by copying the first prompt you generated from chaty PT
    Which                   this prompt describes the scene with the original animals
    OpenWebsite             now open ideogram in your web browser
    FirstPage               you'll see a straightforward interface that guides you through the image creation process
    PastePrompt             once you're in ideograms find the input box where you can paste your copied prompt
    SelectStyle             before generating the image it's important to select the right aspect ratio
    Anlysis                  since we're creating a video that will be short and likely viewed on mobile devices
    SelectStyle             choose the 9:16 aspect ratio this makes your image fit perfectly for platforms like YouTube shorts Instagram or Tik Tok
    WaitForAI               in just a few moments you'll see the first image

    https://youtu.be/pT3ZUZ2DL3g?t=85

    OpenWebsite             now copy a prompt and open a new tab then search for Leonardo Ai
    SingIn            and click on that link now create an account or login
    LeonardoEntry           now click on image generation and turn on the Alchemy button
    SelectStyle             and now change Leonardo diffusion to Absolute reality
    PastePrompt             after that click on the type prompt bar and paste the prompt you copied in chat GPT then click on generate
    PastePrompt             and once it's done copy another prompt and paste there
    NoMoney                 since Leonardo AI will soon ask you for premium let me show you a better way to get highquality AI images for absolutely free forever

    https://youtu.be/pT3ZUZ2DL3g?t=122

    OpenWebsite             open your browser and search for Mid journey.com
    SearchInWebsite         now click on search prompt bar and search for luxury or anything related to what you're making video about
    IThinkGood              I think all this images look really nice since my video is about
    IDownload               I will also download all three richest leaders images since the video I'm making is about them

    https://youtu.be/VOfC13e3oHk?t=479

    OpenWebSite             you need to go to Leonardo Ai and log in
    SelectStyle             then click on anime and choose any style of your choice
    Generate                and click on generate with this model
    PastePrompt             now go back to chat GPT copy a prompt then paste it there
    Generate                and click on generate
    ItLooksGood             everything about this tool is great
    Price                   but the problem is Leonardo AI will ask you to upgrade to premium after generating just three images
    IHate                   and honestly that gets on my nerves

    https://youtu.be/X1wVW-ENssU?t=127

    Step                     step two generating hybrid animal images on leonarda.ai
    Already                 Now that we have our prompts ready
    MyGoal                  It is time to generate images
    IUseToolSimple          we`ll be using the leonardo.ai tool for this part
    IReveal                 If you are new to leonardo.ai, don`t worry, i`ll guide you throught it
    OpenWebsite             start by going to leonardo.ai
    signIn                  and either sign in or sign up if haven`t used it before
    FirstPage               After sign in you are taken to the dash board
    ClickButton             From there click the image generation button to begin
    Trick                   now here gets more tricky
    ...
    SelectStyle             click on preset and choose the illustrative albo model
    Explained               this will give us nice detailed and vibrant images
    SelectStyle             next change the image Dimensions to 2 to three
    Explained                so that the image looks tall and clean
    PossibleOption          now scroll down to the advanced settings
    PossibleGood            most of your images will be generated just fine using the albo Bas XL model
    PossibleBad             but in cases where where that doesn't work as expected you can switch to Leonardo lightning XL for even better consistency 
    IUseToolDetailed        I've selected albo base XL for now
    PastePrompt             let's go ahead and copy our first hybrid animal prompt from chat GPT and paste it into the prompt box here
    WaitForAI               in just a few seconds we've got our first hybrid animal
    Continue                let's keep going with the rest of the prompts to save time
    PickGood                 I'll demonstrate just one more and then we'll proceed that looks good

    ===================================== Music

    https://youtu.be/WXX7g304xNY?t=440

    MyGoal                  since we want our video to get monetized
    PleaseDontDo             we can't just use any song
    PleaseDo                we need one that's copyright free
    OpenWebsite             for this use any website like pixabay
    SearchKeyWord           and search up ancient music and you'll find tons of copyright free songs
    Download                so simply download one you like

    https://youtu.be/Gnjc_0iqXDI?t=158

    Features                sunno AI a tool that can create full songs from a single text prompt
    TrustMe                 I mean let's be real
    ItIsGood                this is some futuristic sci-fi level stuff
    MoreGood                but the craziest part within just 5 months of launching sunno AI raised $125 million in its first funding round yeah

    ItIsGood                most yoube out there will hype soono AI without giving you the full story
    PartGood                they'll tell you it's revolutionary and gamechanging blah blah blah
    PartBad                 but they never explain how you can make money with it but I will
    PayAttention            now pay attention because the next part is the real no-brainer
    CanMakeMoney            sunu AI isn't just some tool it's a money-making machine
    EarnMoney               in fact it's already helping creators rake in thousands of dollars per month some even up to $100,000
    CareYou                 I'm talking about people just like you
    IReveal                 and today I'm going to walk you through exactly how they're doing it
    YouCanDoIt              so you can rep at their success
    IReveal                 but first let me show you whyso AI is the tool you need to be using right now
    Feature                 let's get you familiar witho AI your new best friend for making music without needing years of practice
    NoMatter                 whether you're a beginner or a pro looking to speed things up
    ItIsEasy                sunno AI makes it super easy no music theory or complicated software required
    IReveal                 now I'll show you how to use sunno AI in five simple steps
    PayAttention            don't skip any and you'll be creating songs in minutes even if it's your first time
    Transition              ready let's Dive


    OpenWebsite             In step one head over to Google type in sunno Ai and hit the first link
    OpenWebsite             you see in the search results that's your gateway to an entirely new world of Music Creation
    SignIn                  once you're there you'll need to sign up it's super straightforward
    CreateAccount           just fill in your basic details like your name email and create a password honestly


    ====================================== Voice Over ======================================

    IUseToolSimple          for that we will use 11 laabs
    OpenWebsite             Now search for 11 labs
    SingIn            and sign up or login
    SelectVoice             after signing up you should click the voices button and choose Adam's voice
    PastePrompt             now go back to chat GPT and copy your script that paste it in here

    https://youtu.be/mFZrGTmPSQ0?t=520

    Intro                   let's start off by enhancing The Voice Over
    ItisGood                Look 11 Labs is great
    YouCouldDoBetter        but with a little bit of trimming anyone can make it 10 times better

    https://youtu.be/nk6SG61-Wo0?t=260

    SelectYouWant           just scroll through and and choose a voice you like
    SelectYouWant           11 laps has a mass library of dope ass voices
    PastePrompt             now just copy your script from GPT and paste on a 11 lap script box
    SelectVoice             and then choose your [Music] voice after pasting your script
    SelectVoice             just select the voice you want to use
    ClickGenerate           and hit generate then preview how the script sounds
    Download                and then hit the download icon and then download your new free voice over

    https://youtu.be/WXX7g304xNY?t=311

    ItIsImportant           the quality of the voice over is crucial
    ViewrLikes              I've seen dozens of comments on videos say thank God a human voice for once I'm so tired of these AI voices
    How                     so how can you make a voice over with AI that sounds like a human
    IReveal                 let me show you
    OpenWebsite             first of all go to this website called 11 Labs
    Feature                 it's one of the first AI voiceover websites super cheap if not free and they have pretty realistic voice voices
    YouHaveOption           when it comes to picking a voice you can either find one yourself in the voice library or you can just clone The Voice used in other channels videos
    DownloadVideo           to do this copy the video link from YouTube and download it
    ClickGenerate           then head back over to 11 labs and choose instant voice cloning upload the voice file and click on add voice
    YouHaveAbility          you'll then be able to use this voice on any script


    ====================================== Edit =============================================

    https://youtu.be/VOfC13e3oHk?t=762

    IUseToolSimple          we're going to use one of the best beginner-friendly editing software wondershare film
    Explained               and that's because it currently has all the best AI features
    ForExample              for example you can generate high quality AI images in seconds you can create your own background music of any genre to avoid copyright
    Compare                  and it also offers the best text to video AI tool compared to other AI tools
    OpenWebsite              to get it open your browser and search for wondershare fil Mo then click on that one
    DownloadInstall
    Click                   and click on AI text to video then click on generated by Ai
    SelectStyle             and select what type of text you prefer
    PleaseTypeDetail        and after that type this as your video's topic

    TakeTimes               it will take a few seconds 
    ItisGood                and that is absolutely genius
    Explained               it mentioned all of them and explained a few things about their background

    https://youtu.be/VIu2dzOCRrw?t=285

    AIChange                  but since AI created this there's probably going to be some stuff that you might want to change right
    Steps                   so I'm going to show you the three best ways to improve your videos on this site
    Bad                     after watching our first draft I noticed that the hook is pretty boring
    BadIsImportant          if you don't hook the viewer in the first 30 seconds then you're not going to get views
    WeMustChange             so we need to improve this

    https://youtu.be/mFZrGTmPSQ0?t=504

    IntroPre                 to make things easier when we're going to edit it all together
    IUseToolSimple          I'll do this next part in cap cut
    ExplainFeature          since it's free and has all the editing tools we need
    SelectYouWant           but you can really go with any editing software you want
    IUseToolSimple2         I'll use the free cap cut desktop app but the online version works just as well
    StartProject            and it's also free so start off by creating a new project
    Ready                   now that we have our project ready let's start off by enhancing The Voice Over

    https://youtu.be/mFZrGTmPSQ0?t=527

    Strategy Faster Pace

    Drag                    drag it to the timeline and cut out all the pauses
    OverLap                 then overlap the clips with just a couple of frames
    Repeat                  and repeat this throughout the entire voice over
    Called                  this is called a j cut
    Explained               and it'll give the video a faster pace and make it more engaging
    Check                   when you're done do a quick sound check so it sounds good

    https://youtu.be/WXX7g304xNY?t=456

    SelectYouWant           there's not a particular editing software you need to use
    IUseToolSimple          for this I'll use cap cut
    Feature                 since it's free and has all the features we need
    OpenSoftware            so for cap cut open up a new project
    SelectStyle             and choose 169 as the format


    ================================= Money =============================================

    https://youtu.be/VIu2dzOCRrw?t=65

    AbleMoney               we can see that they are are indeed approved for monetization
    HowMuch                 but how much are they actually making 
    CPM                     to know this we need to get a sense for their cpms
    CPMDetailed             a CPM is the amount that an Advertiser pays per 1,000 views on that channel
    ExactCPM                luckily for them the Carnes has a CPM of around $4
    Math                    so now we just do some simple math
    MatchDetailed            17 million views divided by 1,000 * 4 as you can see they've likely made more than $65,000 already
    InShortTime             and they started this channel just 9 months ago
    Good                    obiousviously not bad

    https://youtu.be/lc4lygDmHbI?t=72

    Unkonwn                 if you're unfamiliar
    Explained               CPM stands for cost per Mila or the amount of money that advertisers pay for every 1 000 views

    ============================== Generate Video  ===========================================

    https://youtu.be/VIu2dzOCRrw?t=160

    Intro                   next we're going to come over to the new AI tool that makes all of this possible
    Called                  it's called invidio Ai
    WantKnow                 and you guys are going to be pretty mind-blown about how automated this process can truly be
    EntryPage               once you're in it's going to take you to this page where you can input a prompt and give a detailed instruction on what we're looking for in our video
    PleaseUsePrompt         and this is the prompt formula that you should use
    ForYourConvenice        I pasted a copy of it below in the description for you so you can basically just plug and play with whatever topic that you personalize
    MyGoal                  with for my video our goal is to make a video as close to the example that got 13 million views as possible
    Prompt                  so I'm going to have it create a 12-minute video
    Explained               and the reason that I want it to be 12 minutes is because once you're monetized you can run midr ads on any video that's longer than 8 minutes
    Explained               meaning you can get double or triple the ad Revenue that a video that's shorter than 8 minutes might get
    TellAI                  next I'm going to tell the AI that this is for YouTube and what we're talking about
    TellAI                  we're going to tell it to use stock footage and this is
    EntryPage               we're just going to click generate video and it's going to bring us to this screen
    ExplainePage            here you can see that they're trying to understand our audience
    WaitAIWorking           then click continue now we're going to let the AI do its thing
    Advantage               no more wasting hours script writing no more looking for stock footage no more hunting down good music in video's AI can do all of this for us

    https://youtu.be/_0nm9_m3yyE?t=237
    
    OpenWebsite             you'll be redirected to this page
    SingIn                  where you can create a free account
    PageEntry               when you enter in video AI the first thing you'll you see is this text box right here
    Explained               and this box is the simplest way to generate videos
    UsePrompt               so I could type for example
    WaitAIWorking           and inv video AI is smart enough to make that happen
    NotBest                 however this is not the best way to generate videos
    MoreAdvancedWay         as this tool offers much better abilities
    FrameWork               so I'm going to share three methods I use myself and the second is actually my favorite

    https://youtu.be/_0nm9_m3yyE?t=424

    ItisGood                so as you saw the video turned out pretty well
    IsGood                  and the fact that we only provided the article link to make this video is actually impressive
    NextStep                but don't forget that this is one out of three methods I'm going to show you
    AIChange                so let's say you don't like something and want to make some changes
    ItisEasy                well that's actually very simple to do

    https://www.youtube.com/watch?v=nsGvN6ZU-rE

    Intro                   it's another great tool for creating short videos like this
    OpenWebsite              visit the hyper aai website
    SignIn                   and log in with your Google account
    FirstWebsite            on the dashboard select image to video upload your images
    ClickGenerate           and click create
    WaitForAI               in just a few minutes you'll have your hybrid animal video ready

    ==================================== Hook =================================

    https://youtu.be/mFZrGTmPSQ0?t=336

    Intro                   in the work I'll guide you in mind when creating a hook
    Detailed                first and foremost we got to trigger curiosity something that makes the viewer feel like they need to know what's about to happen next
    ForExampleVideo
    Explained               you see how a lot of people would feel like the video was talking to them personally which unknowingly makes you think
    Simulate                yeah I can relate to that I'll keep watching this 
    ITried                  so I sat down and used all of my knowledge to come up with this hook

    Intro                   did you	see that
    Talk                    the first line made the viewer wonder what three things they shouldn't be wearing
    Anysis                  so they most likely stayed longer than they'd usually do another thing
    Intro                   to have in mind is to call the viewer out

    ================================ Retention / Watch Time ==================================

    https://youtu.be/lc4lygDmHbI?t=148

    Seceret                 I'm going to let you in on a little secret subscribers really don't matter
    AtLeast                 or at least they don't as much as other platforms
    ExamplePositive         I see YouTube shorts channels that have almost no subscribers with millions of views
    ExampleNegative         and I also see Channels with tons of subscribers who almost get no views
    Summary                 this tells us one very important thing it all comes down to ...
    Whether                 how long you can get the average viewer to watch your video whether you have 1 000 subscribers or 100 000 subscribers
    ItisImportant           the most important metric is watch time
    YoutubeRecommandation   if people watch your videos all the way through through then YouTube will keep pushing your shorts videos until people stop watching
    Proof                   if you want more proof on that we can just look at my own most viral videos and see that ...
    ForYourConvenice        I actually created a viral checklist for YouTube shorts to help you go as viral as possible
    CheckList               it goes like this 
    recommend               I highly recommend that you save this for later
    Explained               because it's what I use every time I create a piece of content
    TrustMe                 and trust me it works
    How                     so again what does all this mean to you in whatever Niche that you've chosen
    IReveal                 well I'm about to show you to find out


    ================================= Topic / Niche ===========================================

    https://youtu.be/NptfUad6HSI?t=82

    Step                    so to start this off step number one is choosing the best Niche
    Explained               and to do so in this process you have to choose a good topic for your channel
    GuruDoWrong             a lot of people go wrong at this point in this process
    ItIsImportant           and I would like to go as far as saying that choosing the right topic or Niche for your channel is actually 50 % of the entire process when it comes to doing
    PartBad                 because if you choose the wrong topic
    AllBad                  you're just going to be barking up the wrong tree
    ItIsEasy                but the good thing is that this can be easily solved 
    ForYourConvenice        I'm going to even give you guys some of the best topics to make sure that you guys choose the right topic as well
    ForExampleDetailed      so the best topics when it comes to doing that on YouTube are basically related to the categories like health
    Include                 and this can also include things like weight loss gaining muscle working out yoga

    Summart                 now these are just some of the best niches or categories for specific reasons 
    Expalined               and those reasons are that there's a lot of demand for these categories and likely always will be
    ItIsGood                so you guys will be building a strong Evergreen foundation for your channel
    MoreGood                that will typically have a better average CPM which is how much you're going to make per thousand views from your ad revenue 
    And                     and you can actually monetize in multiple ways
    Explained               which means you guys can either sell your own product or promote an affiliate product

    https://youtu.be/zYxquOW2_Zc?t=82

    Theory                  no need to break new grounds here instead go with a proven concept people are already making money from
    PleaseDo                go to YouTube and see what people are already interested in
    Thechannel              most popular AI Avatar channels are in the finance and making money online Niche
    Explained               which year after year has proven to be a niche lots of people are interested in
    Notice                  you could start a channel in another Niche but make sure there are channels in that Niche that get a lot of views
    YoutubeAlgorithms       a good sign when looking for a niche is that smaller channels are pushing a lot of views
    YoutubeAlgorithms       this means YouTube is showing their videos to new audiences
    ForExample              like a Channel with 10K Subs getting 100,000 views on multiple videos that's a huge green flag
    PleaseDontDo            also make sure to avoid niches with huge dominating channels in it
    ItIsBad                 if you start a Channel with movie Recaps big channels like watch Mojo will completely Crush you
    Explained               you can't compete with their quality and consistency if you got no budget or momentum

    https://youtu.be/zYxquOW2_Zc?t=260 三层解释，一层理论，一层自己，一层现象

    ItIsImportant           this is without a doubt the most important thing when it comes to going viral on YouTube
    PartGood                 you can have the greatest script and editing ever
    KeyBad                  but if you have a bad topic
    AllBad                  no one will give a about your video
    ExamplePartGood         I posted this video a couple of weeks ago great script and editing
    ExamplePartBad          but barely got any views compared to my other videos
    Explained               simply because it was a bad topic
    ViewerBad               no one in my audience cared about it
    Example                 so spend time finding good topics sometimes you'll see a trash video getting hundreds of thousands of views
    Explained               only because it was a good topic that interested a lot of people
    PleaseDo                go search up your Niche on YouTube and see what's going viral
    IActually               lately I've seen that videos about YouTube automation perform really well so I'll pick that as my topic
    Theory                  now remember you should look at what performs well for others but also what performs well for your specific Channel


    SeleceYouWant           choose a YouTube channel topic that you're either passionate about or already have knowledge
    ISelect                 for example I focused mine on health optimization
    Explained               because at the time I was inspired by popular podcasts like Joe Rogan and Andrew huberman
    SelectYouWant           but feel free to pick any topic that you enjoy
    NoMatter                gaming scary stories history it doesn't matter
    MakeSure                just make sure that it's engaging to a wide audience of people
    RPM                     as your earnings will directly depend on how many views you get            
    
    https://youtu.be/znb3_gFSaac?t=103

    Theory                  the more targeted your Market is the faster your course will sell
    ForExample              for example let's look at AI open up Google Trends
    SetKeyWords             type in AI as your keyword and set the filter to the last 12 months and on the country set for worldwide
    Compare                 now let's check what's hot compare with other topics like supplements on the ad comparison sections
    ItIsGood                you'll probably see ai's blowing up right now
    Niche                   so boom AI it is we got our broad Niche

    WeAreGood               right perfect we've successfully applied the Walt Disney principle
    Word                    that is find something you love and do it better than anyone else

    https://youtu.be/X5mrMO7HPJU?t=267

    ItMatters               choosing the wrong or right Niche on YouTube can make the difference
    Compare                 between a Channel with 6 100 videos but only 150 subscribers and another with only 20 videos but 150,000 subscribers
    Compare                 that's a thousand times the subscribers with just a fraction of the total number of videos
    ItlooksBad              if you're an unknowing YouTuber you might think this is unfair
    ItLooksBad              you put in the same amount of effort and time if not more yet
    ItLooksBad              the other guy gets more views and already bought that new car
    YouHurt                 you can't even hit the first 1,000 subscriber
    ItIsEasy                however the truth is far simpler than you might think
    YouAreWrong             you just may be in the wrong Niche
    GuruDoBad               here's the thing the majority of YouTube channels fail
    FakeReason              and it's not because their videos are that much worse than others
    RealReason              most of the time it's simply because these creators chose the wrong Niche
    IWorkHard               so to make things easier I've spent the past few days and nights diving deep into research 
    ForYouConvenice         to uncover 15 of the most profitable faceless niches that you can start right now
    Because                 because of the number of niches
    IReveal                 I won't be creating demos of each of these videos
    IReveal                 but I'll do my best to show you exactly what you need to create your own

    https://youtu.be/kl9jhLlMtyc?t=126

    IfYou                   if you've ever tried creating a shorts Channel and your videos perform like this
    YouDoWrong              is probably what you're doing wrong
    IfOneBad                if you have a bad video idea
    ThenAllBad              your video won't go viral no matter how good it is
    Theory                  it's like trying to become the greatest McDonald's cleaner in the world you won't get paid more
    MyGoal                  instead make videos about a trending topic or even better implement the trend into your video
    WatchVideo              look at this guy he's selling perfumes which is a trending topic itself
    AnlysisStrategy         but to make it better he starts his videos by mentioning trendy people
    ManyViews               these two videos combined have three million views
    Theory                  the results speak for themselve
    Summary                 where I'm getting at is make videos about topics people are interested in

    =======================================  Head =========================================

    https://youtu.be/bJx7UwrSXcI?t=6

    AmongUs                 among all the passive income options out there
    ItIsEasy                YouTube automation stands out as the best easiest and most affordable
    Important               it's something you can do from anywhere no matter your age and you don't need to spend thousands of dollars
    Important               most importantly you can do everything from home with just a computer and internet access with new AI Tools
    ItIsEasy                automating YouTube has become simpler than ever before
    IReveal                 today I'm sharing a thrilling and rapidly growing Niche
    Detailed                that's taking the Internet by storm hybrid Animal Creations
    YouHaveSeen             you've probably seen some of these incredible videos making waves on YouTube Instagram and Tik Tok
    Detailed                showcasing amazing unique creatures
    Before                  before we get into the video creation process
    Mention                 I need to mention something important
    Check                   when I checked this channel on YouTube's monetization Checker
    Bad                     it wasn't monetized as a result their content isn't earning any Revenue
    ExplainLater             I'll explain the mistake and how you can avoid it later
    IReveal                 in the video in this video I'll show you how to

    https://youtu.be/Gnjc_0iqXDI?t=8

    IReveal                 today I'm going to show you how I make highquality viral monetizable music faceless YouTube automation videos with just two AI tools
    IReveal                 but before that let me share something insane with you
    ThisChannel             this music channel Loi girl 
    EarnMoney               is earning $10,000 to $100,000 
    InShortTime             per month from YouTube AdSense alone
    ThisChannel             another Channel called once upon a time
    EarnMoney                is also earning $10,000
    InShortTime             per month
    WithTool                by using AI generated visuals and music
    TrustMe                 and trust me
    WatchToEnd              if you watch this video till the end
    YouCanDoIt              your dream to make $10,000 per month from a faceless YouTube channel can be true
    IReveal                 now let me show you the power of the music industry
    thisChannel             check out this channel soothing relaxation
    GoToVideo                if we go to its most popular video
    HasViews                we'll see a video with 434 million views
    EarnMoney               can you guess how much money this channel has made with this single video alone
    ReQuestion              and how long it took them to create it first
    ComputeMoney            let's calculate the money
    ComputeMoney            let's take an RPM Revenue per Millie of $1 for every 1,000 views which is the minimum on all of YouTube
    EarnMoney               so from 434 million views they easily earned 434,000
    InShortTime             and it only took them about 3 to 4 hours max to create these videos
    Intro                   what the fu yeah it's true to say that
    EarnMoney               this channel made more than half a million dollars
    InShortTime             in just 3 to 4 hours
    YouCanDoIt              and this isn't a one-time success story on this channel if you look the channel has millions of views on other videos too
    YouCanDoIt              and what if I told you that you could make this happen too
    EarnMoney                maybe not that much but I promise you can make more than enough
    WithTool                just by using two AI tools all right
    WatchLater              now we need your full attention for the next few minutes
    IReveal                 because I'm I'm about to blow your mind today
    IReveal                 I'm spilling the beans on how I make faceless music videos that go viral
    WithTool                using only AI
    IsEasy                  I'm talking about making professional level music with zero musical skills
    IReveal                 and that's not all I'm also going to show you a secret strategy on how to monetize these videos
    IReveal                 and make money from them and you won't even have to show your face
    PayAttention            if you're thinking about skipping ahead don't because if you miss even one second you won't fully understand the magic formula that's about to come your way 
    IReveal                 I'm going to show you how you can make the dream music video like Once Upon a Time channel does
    Research                I'm choosing it because after serious research

    ======================================= Transition ====================================

    Intro                   as later in the video where I'm going to show you all the editing I'll give you the most valuable tip you'll hear this week on how to fix this so keep watching anyways


    W
    https://youtu.be/p7YtqN9C7j4?t=265
     */
}
