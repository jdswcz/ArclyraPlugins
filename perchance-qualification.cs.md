# Kvalifikace Perchance integration by Arclyra na Windows

Tento postup sbírá důkazy k M1/M2 a ručnímu M3/M4, nikoli k celému vývojovému plánu. Skript **neinstaluje**
plugin, neotvírá prohlížeč, neposílá rukopis a nekliká na Generate/Continue. Ruční
kontroly zůstávají samostatné; úspěšný podpis je nenahrazuje.

## Předpoklady a spuštění

- Windows, Git a .NET **8** SDK; CI používá 8.0.425.
- Čistý checkout větve `202610_Perchance`, bez změněných či nových neignorovaných souborů.
- Existující běžné developer signing soubory v `../plugin-keys` vůči kořenu checkoutu:
  `arclyra-developer-private.key`, `arclyra-developer-public.key`, `arclyra-developer.sig`.
  Je to stejné místo a profil jako OpenAI/Ollama. Žádné trusted klíče Arclyra AI.
- Arclyra host 56 nebo novější pro plugin 0.5.0, nezapisovací kontrolu analytického snapshotu, chráněné vytvoření alternativy a inline diff.

Z kořene checkoutu v PowerShellu:

```powershell
.\tools\Test-PerchanceQualification.ps1 -OutputRoot C:\_DATA\TEMPORARY\perchance_qualification
```

Výstup musí být mimo checkout a mimo adresář klíčů. Skript vytvoří novou unikátní
složku běhu, nic existujícího nepřepisuje. Při chybě oprávnění ji řešte běžným
nastavením přístupu účtu; skript nezvyšuje oprávnění ani neobchází politiku systému.
Privátní klíč nevkládejte do repozitáře, balíčku ani zasílaných příloh.

## Co skript provede

1. Ověří větev, čistý stav a HEAD; znovu je ověří během a na konci běhu.
2. Sestaví plugin v Debug i Release s nezměněnými sdílenými packaging/signing targets
   a běžnými externími klíči. Chybějící vstupy, selhání nebo varování build zastaví.
3. Zkontroluje kanonickou strukturu balíčku, přesný název/ID, vstupní DLL, minimální
   host, aktuální textové capabilities a oddělená běžná podpisová metadata. Zakáže
   přibalený SDK, soubory klíčů, traversal/duplicitní cesty a Trusted Component deklaraci.
4. Ověří kopii balíčku přes `Arclyra.PluginSignTool verify-package --developer-name Arclyra`.
   Autoritativní je stávající verifier se stejným trust rootem jako host, nikoli samotná
   přítomnost JSON metadat. Ověří také explicitní odmítnutí nesprávné developer identity.
5. Pozmění pouze vlastní dočasnou kopii a vyžaduje odmítnutí kvůli změně obsahu. Kopii
   odstraní; původní podepsaný balíček nemění. Náhodná chyba procesu se za odmítnutí
   nepočítá: kontroluje návratový kód i konkrétní diagnostiku verifieru.
6. Uloží samostatné Debug/Release balíčky a `qualification.json` s commit SHA, verzí,
   kontrolními součty a výsledky. Neobsahuje klíče, podpisové hodnoty, raw tool output
   ani text příběhu. Stav `automated-checks-passed-manual-gates-open` stále znamená,
   že **ruční kvalifikace není hotová**. Selhání zapíše `failed` a název kroku.

Při chybě buildu spusťte běžný build projektu lokálně pro podrobnou diagnostiku.
Skript záměrně neukládá nezpracovaný výstup nástrojů do protokolu. Nevytváří náhradní
klíče a nemá přepínač pro vynechání podpisu. Ani úspěšný běh nepotvrzuje schválení
distribuce ze strany Perchance.

## Ruční checklist — běžné Plugins UI

Použijte testovací workspace a vlastní nesoukromý text. Nevypínejte reklamy a
neměňte stránku. Neodesílejte vlastní rukopis jen pro kvalifikaci, pokud to nechcete.

- [ ] Nainstalovat ověřený Debug/Release balíček přes **Plugins → Install Plugin**;
  ověřit přesný název a běžnou developer identitu, nikoli Trusted Component.
- [ ] Enable/disable, unload a opětovné otevření okna obnoví původní toolbar.
- [ ] Reinstall/uninstall nepřepíše cizí providery a nenechá staré ovládací prvky.
- [ ] Update skutečně otestovat dvěma odlišnými platně podepsanými verzemi; reinstalaci
  stejné verze nevydávat za test aktualizace. Není-li starší balíček dostupný, bod nechat otevřený.
- [ ] Perchance nahradí celý horní toolbar; Insert Prompt, Import Result
  a Other providers jsou dostupné podle kontextu. Ostatní providery se nezmění.
- [ ] Other providers, odchod ze stránky, změna kapitoly/workspace a zavření/otevření
  okna bezpečně ukončí starý kontext; nic se neimportuje do jiného dokumentu.
- [ ] Prompt nejprve zkontrolovat, vložit explicitním Insert Prompt; existující příběh
  zůstane zachovaný. Generate/Continue ovládat **ručně na viditelné stránce**.
- [ ] Import Result otevře celý náhled; Unicode, odstavce a text mimo viewport se
  zachovají, UI/overview/continuation a označené site summary se nepřimíchají.
- [ ] Save new revision přidá novou revizi; staré a finální verze zůstanou zachované.
  Cancel, prázdný/duplicitní výsledek a změna kontextu během náhledu nic nepřepíší.
- [ ] Pro Text Assistant nejprve otevřít běžné Perchance okno; v editoru vybrat text
  a každou ze šesti akcí. Ověřit editovatelný jazyk/požadavek, samostatné Insert/ruční
  Generate/Import a celý původní/návrhový text. Starý webový příběh odstranit pouze
  ručně z návrhu; plugin ho nemaže ani automaticky neizoluje novou odpověď.
- [ ] Replace captured selection změní pouze původní výběr s běžným Undo a bez
  automatického uložení. Změna dokumentu/revize/výběru, Cancel Text Task, zavření
  editoru a disable pluginu nepřepíší text. Žádné prompt-detail verze se tím nemění.
- [ ] V Details otestovat všech pět typů; v alternativních Versions plný neuložený
  text bez přimíchání metadat Main. Vybrat úlohu/jazyk, zkontrolovat celý prompt,
  samostatně Insert Prompt / ruční Generate / Import Detail a původní/návrhový text.
- [ ] Import Detail bez aktivní detailové úlohy nic nečte ani neimportuje kapitolu.
  Schválení mění pouze zachycený editor; Save / Save changes zůstává samostatné.
  Main ve Versions je read-only; uložené/ostatní verze a name/age/category se nemění.
- [ ] Změna detailu/verze/workspace během náhledu, smazání/revert, Cancel Text Task
  a unload odmítnou starý návrh. Cancel náhledu dovolí retry, souběžný import jen
  jedno použití callbacku; nová fakta nejsou schválena bez výslovného přijetí.
- [ ] U výběru i detailu/verze zvolit Review changes; změny jsou read-only, přidání
  podtržená/tučná a odstranění přeškrtnutá. Editace i změna a vrácení textu musí
  vypnout potvrzení do nového Review changes. Whitespace značky se do výsledku nevloží.
- [ ] Velký rozdíl zobrazí úplné bloky a upozornění na hrubší porovnání, nikoli
  zkrácený text. Cancel a změna zdroje/targetu během review nic nepřepíší.
- [ ] Chyba/skrytý či zamčený editor má srozumitelnou obnovu; toolbar vždy nabízí návrat.
- [ ] Creative Builder v Details/alternativách nabídne všech šest úloh. Zkontrolovat
  jazyk, přesný text zdroje a sdílená pole; neodesílá se automaticky celý příběh.
  U alternativ se nepřidají metadata Main; Generate/Continue zůstávají ruční.
- [ ] Import Ideas zobrazí celý zdroj, editovatelný kandidát/diff a přesný náhled pro
  ruční kopii. Finish without saving nic neuloží, nepřiřadí ani automaticky nekopíruje.
  Stávající editor, Main, uložené/ostatní verze a outline se nezmění, ani při shodném názvu.
- [ ] Import Ideas bez kreativní úlohy nic nečte a nepřejde do kapitoly; Import Detail
  kreativní úlohu odmítne. Cancel/retry, dvě kliknutí, změna zdroje/stránky a navigace
  během všech náhledů neprovedou zápis. Ruční kopie není test automatického vytvoření verze.
- [ ] Na existujícím zdroji explicitně zvolit Create alternative version, zadat jedinečný
  název a v posledním plném náhledu/diffu potvrdit Create unused alternative. Vznikne
  právě jedna nepřiřazená verze s novým ID, nikoli nový detail. Běžný host autosave zůstává
  zachovaný; ověřit návrat po znovuotevření workspace. Main, staré/neuložené verze,
  jejich výběr a přiřazení default/chapter musí zůstat beze změn. Picker nabídne novou verzi.
- [ ] Main/duplicitní název (včetně case/okolních mezer), prázdný či nadlimitní vstup,
  navigace/unload a změna stránky po posledním schválení nic nevytvoří ani nepřepíší.
  Cancel v každém kroku dovolí retry; odmítnutý odeslaný callback vyžaduje novou úlohu.
  Nový neuložený zdroj bez creation callbacku nabídne jen ruční kopii. Žádné implicitní přiřazení.

Do samostatné poznámky uveďte host/plugin verzi, commit a výsledek jednotlivých bodů.
`qualification.json` a tuto poznámku lze předat ke kontrole; nepřikládejte klíče,
browser profil, cache nebo soukromé prompty. Checkboxy plánu uzavírat až podle těchto
skutečných důkazů. Protokol automaticky neupravuje vývojový plán.

## Automatické fixture testy

Ruční M5 Story Analyst kontrola (vlastní nesoukromý testovací text):

- [ ] Ve výběru kapitoly otestovat Literary Critique, Pacing, Dialogue Quality a Suggested
  Continuity Checks. Jazyk/otázky jsou editovatelné; sdílí se pouze výběr, ne zbytek
  kapitoly/guide/verze. Insert a Generate/Continue zůstávají samostatné ruční akce.
- [ ] Import Analysis otevře celý zdroj, editovatelné doporučení a přesný read-only
  náhled pro ruční kopii. Finish without applying nic neuloží, nepřepíše ani nekopíruje.
  Zdůraznit omezený zdroj; modelový výstup není autoritativní audit ani schválený kanon.
- [ ] Změna revize/výběru/caretu/workspace, zavření, unload a změna webu během posledního
  náhledu odmítnou starý výsledek. Cancel dovolí retry; dvojí import nezpůsobí dvojí čtení.
  Import Analysis bez úlohy nemá fallback; Import Detail/Ideas analytickou úlohu odmítnou.
- [ ] Prakticky ověřit, že Story Writer na ručně vloženou analytickou žádost reaguje
  užitečně. Předchozí prózu/continuation autor odstraní z návrhu sám; automatická
  izolace odpovědi se neslibuje. Character Chat adapter/profilové vkládání zatím není
  implementované; pouhý průzkum jeho formuláře není ruční kvalifikace generování.

```powershell
.\tools\Test-PerchanceQualificationContract.ps1
```

Windows CI spouští 32 deterministických kontraktových případů a syntaktickou kontrolu
skriptů. Syntetická metadata nejsou platné podpisy a fixture výsledek nikdy neoznačí
kryptografický podpis za ověřený. Tyto testy nejsou důkazem přítomnosti operatorových
klíčů, skutečného Debug/Release podpisu, instalace ani živého CefSharp round tripu.

### Opravy 0.5.1 / host 57

- [ ] Tlačítko Preview Prompt chybí; Insert Prompt stále nabízí editovatelnou kontrolu.
  Kompaktní tlačítka mají viditelné vnitřní odsazení a nerostou s výškou popisku kontextu.
- [ ] Vložení/import fungují i při načítání reklamních rámců.
- [ ] Po Insert Prompt a ručním Generate se průběžná próza objeví v návrhu Arclyry.
  Samotný stream nevytváří uložené revize. Import po dokončení a explicitní potvrzení ano.
- [ ] Stop preview, ruční úprava návrhu, změna kapitoly/projektu, navigace a disable
  ukončí stream; zachovají poslední návrh a neupraví jiné kapitoly.

### Opravy 0.5.2 / host 58

- [ ] Insert Prompt funguje na aktuální stránce i s iframe src obsahujícím
  __generatorLastEditTime a načteným dokumentem bez query; po ručním Generate
  se návrh přenáší do Arclyry a po dokončení lze importovat novou revizi.
- [ ] Název provideru je Perchance. Other providers → Perchance tools vrací
  pluginový toolbar bez reloadu; jiné providery/stránky ani disable jej neaktivují.
- [ ] V Details a alternativních Versions je jedno AI tools menu pro všechny
  registrované akce; Save zůstává vedle. Main/obsazený editor jsou chráněné.


### Opravy 0.5.3 / host 59

- [ ] Vložit víceřádkový prompt z Windows (CRLF i samostatné CR); potvrzení nehlásí chybu,
  přehled stránky obsahuje shodný text s LF a objeví se Stop preview.
- [ ] Před begin story je editor skrytý; stream čeká a po ručním spuštění generování se
  zobrazí průběžný text v Smart Builderu. Bez automatického klikání na Generate.
- [ ] Start streaming připojí existující i právě generovaný text bez dalšího vložení promptu.
- [ ] U dlouhého příběhu běží náhled posledního segmentu s pořadím a cílem cca 2000 slov.
- [ ] Split into chapters: změnit cíl 200–10000 slov, Update split, prohlédnout každý
  segment, Cancel i Save chapters. Spojení segmentů zachovává celý text včetně Unicode.
- [ ] Save chapters přidá revizi do původní kapitoly a další kapitoly hned za ni; staré/final
  revize a následující kapitoly zůstávají. Vybere poslední importovanou kapitolu.
- [ ] Opakovaný import stejného prvního segmentu nevytvoří duplikáty. Změna stránky,
  kapitoly, projektu nebo uživatelského textu během čtení/review bezpečně zastaví zápis.


### Porovnávání a obousměrné změny 0.5.4 / host 60

- [ ] Připojit příběh s více uloženými kapitolami a aktuální novou kapitolou. Smart Builder
  obsahuje jen aktuální text, ne předchozí kapitoly ani rozdělení podle 2000 slov.
- [ ] Připojit starší revizi v Perchance a novější pracovní revizi v Arclyře; Sync chapter edits
  ukáže úplný rozdíl před prvním zápisem. Cancel nezmění web ani Arclyru.
- [ ] Po potvrzení upravit uloženou pracovní kapitolu i aktivní nesaved Smart Builder text.
  Web aktualizuje pouze příslušné rozpoznané úseky; okolní kapitoly a mezery zůstávají.
- [ ] Během generování úpravy čekají. Současné úpravy stejné kapitoly na obou stranách
  vyžadují nové porovnání; úpravy v různých kapitolách se zachovají.
- [ ] Automaticky zobrazené chunky se neposílají zpět jako uživatelské úpravy.
- [ ] Import Result uloží jen vybranou rozpoznanou kapitolu; nejasná hranice import odmítne.
- [ ] Duplicitní/reordered text, změna seznamu kapitol, navigace či neúspěšné potvrzení zápisu
  nezpůsobí přepsání lokálního textu. Paměťové SUMMARY bloky se nemění ani neodstraňují.
- [ ] Stop preview/odpojení ukončí i automatický zpětný přenos. Žádný krok nekliká Generate.
- [ ] Save new revision vyčistí pracovní náhled bez smazání textu na webu a bez opětovného
  naplnění náhledu při nezměněném výstupu; úmyslné smazání v editoru se stále liší od Save.

### Předání požadavku z AI tools 0.5.5 / host 61

- V Details i v alternativní Versions otevřete postupně Perchance revizi a Creative Builder.
- Upravte text požadavku (včetně českých znaků a více řádků), potvrďte Insert Prompt a případné nahrazení přehledu. Text musí být ihned v Story AI overview, bez dalšího kliknutí v toolbaru. Původní story se nesmí smazat.
- Generování spusťte ručně na webu. Import Detail musí nabídnout původní text a aktuální návrh a po schválení změnit pouze zachycený editor. Import Ideas musí zachovat svůj tok kopírování/vytvoření alternativy.
- Zrušení review, odmítnutí nahrazení, přepnutí editoru/provideru, navigace, zavření okna a chybný iframe nesmí vložit text ani blokovat nový pokus starou úlohou.
- Během těchto kroků nesmí proběhnout automatické Generate/Continue ani uložení Arclyra revize.

Deterministické testy nenahrazují tento nativní Windows/CefSharp průchod.

### Detail workflow 0.6.0 / host 62

- Bez otevřeného AI Generation Window musí AI tools ihned oznámit chybějící prohlížeč, bez vstupních dialogů.
- Z Details a alternativní Versions vyzkoušejte jediný formulář: Task, Output language, Additional instructions a kompletní request. Cancel nic nevloží.
- Insert Prompt přepne existující Perchance okno na Text Rewriter pro revizi, Text Generator pro Creative Builder. Ověřte vstup/instrukce a zachování starého výstupu. Generování spusťte ručně.
- AI tools musí být znovu dostupné ještě před importem. Vyzkoušejte Cancel pending AI task a zahájení jiné úlohy; starý výsledek se nesmí aplikovat a nový task nesmí zablokovat pozdní dokončení starého.
- Import Detail / Ideas musí číst pouze responseEl a dodržet původní review/diff/destination/save pravidla. Změna vstupu nebo instrukcí na webu musí import zastavit.
- Přepnutí cizího provideru, načítání reklam, změna editoru, zavření okna a navigace během přepnutí stránky nesmí způsobit zápis do jiného cíle.
- V review okně i ve formuláři zkontrolujte čitelné padded buttons při zvýšeném DPI a větším fontu.

Tento nativní Windows/CefSharp průchod zůstává otevřený.
