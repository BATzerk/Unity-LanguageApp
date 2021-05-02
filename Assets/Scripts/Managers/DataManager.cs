using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager {
    // Properties
    public StudySetLibrary library;
    //public List<StudySet> studySets=new List<StudySet>();


    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public DataManager() {
        ReloadStudySetLibrary();
    }
    public void ReloadStudySetLibrary() {
        string jsonString = SaveStorage.GetString(SaveKeys.StudySetLibrary);
        library = JsonUtility.FromJson<StudySetLibrary>(jsonString);
    }
    public void SaveStudySetLibrary() {
        string jsonString = JsonUtility.ToJson(library);
        Debug.Log(jsonString);
        SaveStorage.SetString(SaveKeys.StudySetLibrary, jsonString);
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void ClearAllSaveData() {
		// NOOK IT
		SaveStorage.DeleteAll ();
		//ReloadLevels ();
		Debug.Log ("All SaveStorage CLEARED!");
	}
    public void ReplaceAllStudySetsWithPremadeHardcodedOnes() {
        //// TEMP!
        //List<Term> set1 = new List<Term>();
        //List<Term> set2 = new List<Term>();
        //List<Term> set3 = new List<Term>();
        //List<Term> set4 = new List<Term>();
        //List<Term> set5 = new List<Term>();
        //List<Term> set6 = new List<Term>();
        //List<Term> set7 = new List<Term>();
        //List<Term> set8 = new List<Term>();
        //List<Term> set9 = new List<Term>();
        //List<Term> set10 = new List<Term>();
        //List<Term> set11 = new List<Term>();
        //List<Term> set12 = new List<Term>();
        //List<Term> set13 = new List<Term>();
        //set1.Add
        //cards0.Add(new Term("House", "Hus", "hoos"));
        //cards0.Add(new Term("Car", "Bil", "beel"));
        //cards1.Add(new Term("Green", "Grøn", "grøn"));
        //cards1.Add(new Term("Blue", "Blå", "blå"));
        //cardsNumbers.Add(new Term("Zero", "Nul", "nul"));
        //cardsNumbers.Add(new Term("One", "En", "en"));
        //cardsNumbers.Add(new Term("Two", "To", "to"));
        //cardsNumbers.Add(new Term("Three", "Tre", "tRe"));
        //cardsNumbers.Add(new Term("Four", "Fire", "feeah"));
        //cardsNumbers.Add(new Term("Five", "Fem", "fem"));
        //cardsNumbers.Add(new Term("Six", "Seks", "seks"));
        //cardsNumbers.Add(new Term("Seven", "Syv", "sYv"));
        //cardsNumbers.Add(new Term("Eight", "Otte", "otte"));
        //cardsNumbers.Add(new Term("Nine", "Ni", "nee"));
        //cardsNumbers.Add(new Term("Ten", "Ti", "tee"));
        //library.sets.Add(new StudySet("Colors", cards1));
        //library.sets.Add(new StudySet("Numbers", cardsNumbers));

        library = new StudySetLibrary();
        library.sets = new List<StudySet>();
        library.sets.Add(new StudySet("Danish M2 #1",
            "Selvfølgelig [seføøli] - Of course\nHvor dejligt! [die-leit] - How lovely!\nFjollet [fjolD] - Silly\nUnderlig [å-na-li] - Weird/strange\nForskellige [fu-skæ-lee] - Different\nJeg er den eneste amerikaner [ae-neste am-Ree-khay-nah]. - I'm the only American.\nDet giver mening [gee-ya mæ-ning] - That makes sense\nJeg tog [to] module 1 for fem måneder siden. - I took module 1 five months ago.\nSamtale [sahm-tay-lih] - Conversation\nJeg blev [ble] født i New Jersey. - I was born in New Jersey.\nDa jeg var barn... [daYAva ba'n] - When I was a kid...\nJeg har hørt, at italiensk er svært. - I've heard that Italian is difficult.\nMøder [møD-a] - To meet\nHold da op! - Oh, boy!\nEr du også lige kommet? - Have you just arrived, too?\nVi snakket i tre timer. - We talked for three hours.\nJa, det kan man godt sige. [ka] - Yeah, you could say that.\nFor pokker! [po-ga] - Damn!\nHente [hen-d(e)] - Fetch\nAh, så løber jeg. - Ah, I gotta run.\nHvordan fik du den idé? - How did you get that idea?\nFordi jeg gerne ville prøve at bo her. [gærn vil prø-ah] - Because I wanted to try to live here.\nTidligt [tiD-leet] - Early\nKeder du dig? [kiD-a du-da] - Are you bored?\nHvad lavede du egentlig, før du kom hertil? [ve laveD du æ'ng-lee før du kom her-tih] - What did you actually do before you came here?\nHar du haft din egen [ai-n] bil? - Did you have your own car?\nJeg lavede mine egne [ai-neh] videospil. - I made my own video games.\nJeg havde mit eget firma. [heD mit ahD fee-erma] - I had my own company."
        ));
        library.sets.Add(new StudySet("Danish M2 #2",
            "Jeg kunne [ku] godt tænke mig at tage til [ta tih] Bornholm. - I would like to go to Bornholm.\nUdtryk - Expression\nTaknemmelig - Grateful\nHvad betyder det? [be-tyD-a] - What does that mean?\nMest [may'st] - Most\nMindre [min-dRa] - Less\nBedre [biDa, maybe a ghost of an R] - Better\nVærre [væ-ah] - Worse\nDen værste [værst-eh] - The worst\nFor det meste [fuh de mæ'steh] - Mostly\nLiv [lee-yu, but very quick] - Life\nForhåbentlig [for-håb-ntlee] - Hopefully\nVi talte [tæl-deh] for det meste [de mæ'ste] på dansk. - We mostly spoke Danish.\nJeg forstår ikke helt. [hæ-lt] - I don't totally understand.\nJeg glemmer [glæm-ah] - I forget\nJeg glemte [glem-t(d)e] - I forgot\nKan du gentag? [kadu gentæ] - Can you repeat?\nGlem det. - Never mind.\nSidste gang - Last time\nFem måneder siden - Five months ago\nJeg studerede for at lære [stu-dæ'-D før-a læ-ah]. - I studied in order to learn.\nBesøgte [be-søgd] - Visited\nJeg kan godt lige Danmark på grund af danskerne. - I like Denmark because of the Danes.\ni halvandet år [hal-lanneD år] - For a year and a half\nNemlig - Exactly\nFør i dag, vidste jeg det ikke. - Before today, I didn't know.\nFør han fik jobbet [jobbeD] i Århus, arbejdede [ah-bi-dD] han som kok i Rom. - Before he got the job in Århus, he worked as a chef in Rome.\nVælg. [væl-je] - Choose.\nDe boede i Danmark, fordi de gerne ville prøver noget nyt. - They lived in Denmark because they wanted to try something new.\nHvis det ikke er sjovt, så gør jeg det sjovt, ellers tager jeg en pause! - If it's not fun, I'll make it fun, or take a break!\nHvad hvis jeg gjorde [gyort(d)] det? - What if I did?"
        ));
        library.sets.Add(new StudySet("Danish M2 #3",
            "Resultatet [re-su-táy-diD] betyder ikke noget. Bare have det sjovt. - The result doesn't matter. Just have fun!\nLignende [lee-neh] - Similar\nHan så sport. - He watched sports.\nDet gik godt! Jeg tror, ​​han gik ud. [geek] - It went well! I think he went outside.\nDet skal jeg lære! [de ska-ja-læ-ya] - I need to learn that!\nDet er så fedt med vores nye køkken. Så du står i køkken? - It's so cool with our new kitchen. So you're in the kitchen?\nJeg vil øve mere! [ja vi øø(a) mee-ya] - I'm going to practice more!\nOpgave [op-gæo] - Task\nOpgaver [op-gæowah] - Tasks\nHan er lige gået ned i kiosken. [gåeD niD i kioskn] - He's just gone down to the kiosk.\nFor sent [fuh saint] - Too late\nLov mig, at du ikke bliver mærkelig. [Lo-ma, adu ik blee mæaklee] - Promise me you won't be weird.\nSpiludvikler - Game developer\nLykkes [lygis] - Successful\nKender du det? - Know what I mean?\nSkidegod [skiD-go] - Damn good\nSkønt [skønt] - Great/wonderful\nMå jeg bede om en øl? [må-ja béom en øl] - Can I ask for a beer?\nVent lige lidt - Wait a second\nJa, det har vi klaret. [klaD] - Yeah, we have done that.\nGod pointe. [go po-eng^-deh] - Good point.\nGrunde [gRo-neh] - Reasons\nNyttig [ny-dee] - Useful"
        ));
        library.sets.Add(new StudySet("Danish M2 #4",
            "Hvordan fik du den idé? [Hvordán fikdu dén idée?] - How's you get that idea?\nSkal jeg ikke lige hente et par øl? [Skayíkleehénd-et parø'l?] - Shall I not just fetch two beers?\nButikken [butígn] - The store\nArbejdede [ah-bi-dD] - Worked\nJeg skal sig dig noget. [jaska séeda noD] - I'm going to tell you something.\nBedre [biD-ah] - Better\nHvornår flyttede du egentlig til Danmark? [hvornå' flýdDdu-énlee t-Dánmark?] - When did you actually move to Denmark?\nMen hvor længe skal du egentlig være her? [Menvor læ'ng skaduenglee væ' ha?] - But how long will you actually be here?\nKontrakt [kontRakt] - Contract\nOptagelse [op-tael-se] - Recording\nForkert [fu-kæ(r)t] - Wrong\nDet har du sagt [de há du-sa(u)gt] - You said that.\nEr du så glad for at være her? - Are you happy to be here, then?\nMen er der ikke noget, du savner fra Finland? - But isn't there something you miss from Finland?\nNæ, ikke rigtig. - No, not really.\nLivet her i Danmark er meget som i Finland. [lée-uD her i Dánmark er máhD som-i Finland] - Life here in Denmark is a lot like Finland.\nMen hvad med dit liv her i Danmark? [men veD-me déet lee-uD her i Danmark] - But what about your life here in Denmark?\nDet er noget helt andet end i USA. [de enoD héet anneD i USA] - It's something completey different than in USA.\nTættere på [tædda på] - Closer to\nVejret [ve-yuD] - The weather\nHvis du synes, det er koldt i Danmark, så vent, til du kommer til Finland. - If you think it's cold in Denmark, just wait until you get to Finland.\nNej, det var mest, fordi... [deva méest fodi] - No, it was mostly because...\nNaturen [na-too' ahn] - The nature\nMen jeg kunne godt tænke mig at bo tættere på min familie. [Me ja ko godt tænk ma-a-bo tæda(e) på min familie] - But I would like to live closer to my family.\nDa jeg begyndte at arbejde her [Daya begynd-a a arbejd her] - When I began to work here\nStolt [sto(uh)lt] - Proud\nMen nu må vi se... [me-nu mó vi se] - But let's see..."
        ));
        library.sets.Add(new StudySet("Danish M2 #5",
            "Det gjorde vi allerede. [de gjóahvee aleReD] - We did that already.\nJeg havde det sjovt! [heD(-ah)] - I had fun!\nLæste du den bog, jeg sendte dig? [læste du-den bo, ja sendt(e) da?] - Did you read the book I sent you?\nHåber at du får en hyggelig aften. [as spelled] - Hope that you have a nice evening.\nBrug din fantasi. - Use your imagination.\nHan spillede også tennis. [spil-eD] - He also played tennis.\nTrist [tReest] - Sad\ni stedet for [i steDet'] - Instead of\nJeg fik mange venner. [fick] - I made many friends.\nTør [tø(r)ah] - Dry\nDet er den slags fyr, jeg er. [slahgs fY(r)ah] - That's the kind of guy I am.\nForskel [fo(w)r-ske] - Difference\nJeg kedede mig. [kiDD(th)] - I was bored.\nHvor flyttede du hertil? [flY-deD(-th) du ha-til] - Why did you move here?\nTidligere i dag sagde min ven... [tiD-lee-ah i dag sayy meen ven...] - Earier today, my friend said...\nJeg tilføjer dem senere. [tíl-føjah] - I'll add them later.\nJeg tilføjede dem til sætningerne. [til-føeD; saétning-ah-n(eh)] - I added them to the sentences.\nTidligere [tiD-lee-ah] - Earlier\nTættere på [tehdda på] - Closer to\nKan du forklare? [kádu fo(r)-klaah] - Can you explain?\nForvirret [fuh-vee-u(h)D] - Confused\nHjerne [yuær-neh] - Brain\nDe kom hertil for at opleve livet og mentaliteten i Skandinavien. [fo-a op-læwe-leewD o mentali-té-n i Skandinavien] - They came here to experience the life and mentality in Scandinavia.\nTørsdig [tørus-dee] - Thirsty\nFlyttede [flY-deD(-th)] - Moved (pronunciation)\nhelt andet end i USA [het annuhD enni USA] - completely different than the USA\nIngenting - Nothing"
        ));
        library.sets.Add(new StudySet("Danish M2 #6",
            "[Keelomáyy-da] - Kilometer pronunciation\nSikke en sjov historie! [sig(ge) en sjov hih-stor-i-ah] - What a funny story!\nEr det det samme som...? [de sámm(e) som] - Is that the same as...?\nEr det lige som...? [er de lee sum...] - Is that similar to...?\nEr det (lidt/næsten) ligesom på engelsk? - Is that (almost) the same as in English?\nDet kan (jo) ske. - It happens. (As in, \"It's all right... it happens.\")\nØv! [Øw] - Darn!\nDet var ærgerligt. [a-ouwlidt] - What a shame / I'm sorry to hear that\nEt grimt ord [gRemd] - A bad word\nHar du tid til et spil? [tiD ti ah spil] - Do you have time for a game?\nHvad er forskellen på de to? [hve fo(r)-skellen på dee to?] - What\'s the difference between the two?\nDet lige sket. [de lee skate] - It just happened.\nFormelt [fo-melt] - Formal\nJeg har masser. [ma(e)ssa] - I have plenty.\nLivet er godt. [lee-wull, short wull] - Life is good.\nDu lever livet farligt! [du læwa-leewD fah-leet] - You live life dangerously!\noplave livet [op-le-wah lee-wiD] - experience life\nJeg finder ud af det med tiden. [úD er-day meh-tíDen] - I\'ll figure it out over time.\nJeg var helt tabt. [as spelled] - I was totally lost.\nEr du klar over, hvor sej du er? - Do you realize how cool you are?\nHvor længe skal du så være her? [voe læng skádusa væ-ah ha] - How long will you be here, then?\nIngen [inng] - Nobody\nTypisk [tYpisk] - Typical\nTørsdig [tør(u)s-dee] - Thirsty\nBlive ved. [bleeu viD] - Continue."
        ));
        library.sets.Add(new StudySet("Danish M2 #7",
            "Undrer mig over - I wonder\nJeg havde ret. Du tog fejl. [heD Rat. du to fai] - I was right. You were wrong.\nSkal vi se det? [skavi sæ de?] - Shall we watch it?\nDet har jeg glemt. - I have forgotten.\nJeg er lige blevet [bliD] færdig med at spille. - I just finished playing.\nSpeciel [Speshee-el] - Special\nOg hvad så? - So what?\nFalsk [faelsk] - False\nModig [muDee] - Brave\nLøgner [løin-a] - Liar\nJeg er enig. [æ-ni] - I agree.\nJeg er uenig. [oo-æ-ni] - I disagree.\nJeg er frisk! - I'm down! / Sign me up!\nDe fleste af dem var ret gode. [dee flæyst-e adem va Rat goeD] - Most of them were pretty good.\nKys og kram. - Kisses and hugs.\nDet er op til dig. - It's up to you.\nJeg bliver rasende. - I'm getting furious.\nHold kæft. - Shut up.\nOnde [o(e)neh] - Evil\nBare sådan [baahh súddin] - Just like that\nNyd det, mens det varer. [nYD de, mens de vaaa] - Enjoy it while it lasts.\nHvordan har du det i hjertet? [ee-ah-diD] - How do you feel in your heart?\nHar du set det? [sæy de] - Have you seen it?\nHvordan føles det? Føles det godt? [as spelled] - How does it feel? Does it feel good?\nDet er alt du skal vide. - That's all you need to know."
        ));
        library.sets.Add(new StudySet("Danish M2 #8",
            "Jeg har lært [læaht, fast] - I have learned\nDet er jeg med på! [deya meh på] - Count me in!\nJeg fandt dig! [fan-t da] - I found you!\nAt skifte - To change\nEr det en almindelig [al-méh-n-lee] sætning? - Is that a common phrase?\nJeg troede, det var ret godt! [ja t(R)oh-eD] - I thought it was pretty good!\nKontakte [kohn-tAch-de] - Contact (verb and noun)\nHvorfor smutter du? [smooda] - Why are you going away? / Why are you hopping away?\nJeg skal lige smut i kiosken. [smoot] - I'm just gonna hop down to the kiosk.\nJeg er for de mest interesseret i... [intRuh-sæ(h)uD] - I'm mostly interested in...\nDet vil nok være/blive... [væ-yaa / blee] - It'll probably be...\nSådan én som dig [suddin é'n som-da] - Someone like you\nOg hvad så? - So what?\nFor nylig - Recently\nJeg lærte for nylig... [lær-de fu-nY-lee] - I learned recently...\nLad være. Seriøst. [La væya. Se(r)i-øst] - Do not. Seriously.\nJeg har fundet [fu/o-niD] - I have found\nSiden sidste gang - Since last time\nIndenfor [inn-fuh] - Inside\nUdenfor [uDin-fuh] - Outside\nVi har det på samme måde. [sa-mme mooD] - We feel the same way.\nBeslutter - To decide\nVi sås i går. [sås i gå(r)] - We saw each other yesterday.\nNu skal jeg skifte til løbetøj. - Now I have to change into running clothes."
        ));
        library.sets.Add(new StudySet("Danish M2 #9",
            "Jeg har lyst til det. - I want it (in a lustful want-to-have way)\nJeg er ikke sikker. [sih-gah] - I'm not sure.\nLad os bare gør det. - Let's just do it.\nKan du mind mig om...? [kadu mín-ma om] - Can you remind me about...?\nTak for din tålmodighed. [tål-moo/uDiheD] - Thanks for your patience.\nBare rolig! Det skal nok gå. [bah (r/R)olee, very soft R, almost W. de ska no(k)gå.] - Don't worry! It'll be okay.\nEn af mine kollegaer [e'n a-meen kohlayya] - One of my colleagues\nJeg tænkte [tænk-de(h)] over det. - I thought about it.\nHan skrider [slow: skR(i)eeh-Dah. fast: skRiDah] - He slips/He leaves\nHan skride [skRiDD, tongue comes forward] - He slipped/He left\nOprigtig [op-Rie(k)-dee] - Sincere\nJeg nød det. [ja nøD de] - I enjoyed it.\nPraktisk [pRaktisk] - Convenient\nHøflig - Polite\nDet ligner lort. [leena] - That looks like shit.\nDet ligner et glas vin! - That looks like a glass of wine!\nVi får se. - We'll see.\nLad os se. - Let's see.\nFor en sikkerheds skyld. [sigga-hiDs skYL']. - Just to be safe. / Just in case.\nJeg er lige begynde [begYnt] på min dansk rejser [Rej-se]. - I have just begun my Danish journey.\nLort - Shit (feces)"
        ));
        library.sets.Add(new StudySet("Danish M2 #10",
            "Jeg har taget [tehD] tre jakke/bøger med. - I brought three jackets/books.\nJeg har taget [tehD] tre jakke på. - I have put three jackets on.\nJeg har taget min vand. [Ja ha tehD min vand] - I brought my water.\nDet er det nok. [de-day nok] - It probably is.\nSandsynligvis [san-sY*n-lee-vees] - Probably (one word)\nJeg vil ikke tilføje hvert nyt ord. - I won't add every new word.\nHvilken foretrækker du? [fo(r)e-tRægka] - Which do you prefer?\nEr der faktisk/egentlig et sted i nærheden? [et stehD i næ(r)-hiD'n] - Is there actually a place nearby?\ni nærheden [i næ(r)-hiDn] - Nearby\nikke så langt herfra [lahngt heh-f(R)a] - not too far from here\nMit vasketøj løber jo ingen steder. - My laundry isn't going anywhere.\nRet tæt på [Rat tæt på] - Pretty close to\nEr der (egentlig) en sø her i nærheden? [i næ(r)-hiDn] - Is there (actually) a lake nearby?\nMin yndlingsfarve [Y*n-lings-fauww] - My favorite color\nMin yndlingshund [Y*n-lings-hUn'] - My favorite dog\nHvor dumt! - How silly/dumb!\nDer ligger faktisk en biograf ikke så langt herfra. [lahngt heh-f(R)a] - There's actually a movie theater not too far from here.\nMåne [må/oon*-eh] - Moon\nSide [siD] - Page\nVenstre side [venstRa siD] - Left side\nUndertekster [u/åna-tehkstah] - Subtitles\nOversætte [oVa-sædde] - Translate\nOversættelse [óVa-sæddel-se] - Translation\nHeldigvis [hæ*l-divees] - Fortunately/Luckily\nLige nu er jeg i gang med at designe et computerspil. [méya des-ái-neh] - Right now, I'm in the process of designing a computer game."
        ));
        library.sets.Add(new StudySet("Danish M2 #11",
            "Hvad laver du så for tiden? - So what are you doing at the moment?\nHeller ikke mig - Me neither\nJeg elsker det indtil videre. [ínt-ih víD-uh] - I love it so far.\nIndtil videre går det skidegodt. [ínt-ih víD-uh] - So far, it's going shittin' good.\nHar du noget imod hvis vi bliver forelsket? [nuD imoD viss fa/o-elskD] - Do you mind if we fall in love?\ni forhold til - Compared to/In regards to/In relation to\nForhold [fó(u)r hu(l)d] - Relationship\nSelvføleglig! Klart! Fint! - Sure/Okay!\nMagi [maGEE] - Magic\nJeg kan skrive. [skR-EE-euw] - I can write.\nJeg har skrevet. [skRæ-wuD, just swallow the whole thing in there] - I have written.\nI går skrev jeg [skRéh(ww), 1 syllable, glottal stop at end] - Yesterday, I wrote\nPasse det kl 14? - Does 14:00 work?\nPasser det hvis vi mødes i morgen? - Does it work/fit if we meet tomorrow?\nPasser det hvis vi mødes ved 19-tiden? - Does it work/fit if we meet at 19-ish?\nJa, det passer fint. - Yeah, that works/fits.\nPasse en baby [baby] - Take care of a baby\nDen her jakke passer (til) mig! - This jacket fits me!\nPasser det? - Does it fit?\nDe passer fantastisk sammen. - They fit together wonderfully.\nJeg vil gerne have noget løbetøj der passer til min hunds løbetøj. - I would like some running clothes that match my dog's running clothes.\nVi har fået mange venner. [fåeD] - We have made many friends.\nJeg har set det. [sæt/sate deh] - I have seen it.\nDet er endnu bedre. - That's even better.\nDet sidste spørgsmål - The last question"
        ));
        library.sets.Add(new StudySet("Danish M2 #12",
            "Det er da godt. - Well that's good.\nAt blive forelsket i [a-blee-ah fu-élskeD] - To fall in love with\nJeg blev bare forelsket i dansk. [ble-baah fu-élskeD] - I just fell in love with Danish.\nHele tiden [hæle tiD'n] - All the time\nEndnu bedre end - Even better than\nSikke en overraskelse! [ówa-(R)askul-se] - What a surprise!\nDe er begge gode. [dee er begge gohh(D)] - They're both good.\nJeg kan godt lide dem begge lige meget. - I like them both equally.\nHvorfor ikke begge? - Why not both?\nNervøs [nær-vøøs] - Nervous\nHvad skete der? [veh skáyy-diddáh] - What happened?\nDin hjerne er større end Texas! [støahh] - Your brain is bigger than Texas!\nSå få som muligt [så få^ som múl-eet] - As few as possible.\nMindste to - Only two\nHar du flere spørgsmål? [flee/æ-ah] - Do you have (any) more questions?\nDer var langt færre skeletter i går. [dava lahngt fæ-ah] - There were way fewer skeletons yesterday.\nJeg kan ikke lade være med at synge. [lavæah] - I can't stop myself from singing.\nDe seneste nyheder - The latest news\nEr du i humør til en joke? [(h)umør, like mmmwauh] - Are you in the mood for a joke?\nUndtagen [und-tay'n] - Except\nBesat af [bihsátt ae] - Obsessed with\nImponeret [im-po-næ-uD] - Impressed\nDer var meget få problemer. - There were very few problems.\nJeg tænker på dig hele tiden. - I think about you all the time.\nJeg forstod alt undtagen... [ja forst-uhD alt und-tay'n] - I understood everything except...\nJeg er lidt imponeret over mig selv. [im-po-næ-uD óVa ma-sel'] - I'm a little impressed with myself."
        ));
        library.sets.Add(new StudySet("Danish M2 #13",
            "Du har min opmærksomhed. [op-mæ^-rksom-hiD] - You have my attention.\nFor at udfordre mig selv. [oouD-fådRah] - To challenge myself.\nFor at give mig selv en udfordring. [fo-a gEE ma sel en úDfordRing] - To give myself a challenge.\nBemærke [be-mæærge(h)] - To notice\nFolk plejer at gå med til det. - People usually go with it. (As in, cooperate)\nDet plejer folk at acceptere. - People usually accept it.\nSkal jeg tage noget at drikker med? - Should I bring something to drink?\nEr der noget specielt du kan lide? - Is there anything special you'd like?\nHamrende godt [hahm-Re-ne, or hammena] - Hammeringly good\n(Det går) kanon. [kanón] - It's going really well. (Like a cannon)\nDet er enten A eller B. [end'n] - It's either A or B.\nEnten eller. [end'n] - Either or.\nStemme - Voice\nHøj stemme - Loud voice\nJeg er glad for at du spurgte! [fo-a-du spour-deh] - I'm glad you asked!\nHar du spurgt hende? [hadu spówutt hennn] - Have you asked her?\nhvis du foretrækker vil det, ... [fow(ah)-tRaggedeh] - if you'd rather, ...\nSiger man bare...? - Do you just say...?\nJeg er ikke bange for at laver fejl. [fai-l] - I'm not afraid of making mistakes.\nJeg holde (virkelig/meget) af dig. [ja hola adda] - I care about you.\nJeg har et sang en mit hjerte. [(y)eaa-deh] - I have a song in my heart."
        ));

        SaveStudySetLibrary();
    }



}


