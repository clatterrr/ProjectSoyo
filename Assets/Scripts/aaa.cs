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


    ========================================= Image ===========================================

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




    W
    https://youtu.be/p7YtqN9C7j4?t=265
     */
}
