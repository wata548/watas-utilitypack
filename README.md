# Wata's Utilities
이 라이브러리는 개발에 편리한 기능을 지원합니다.

## 기능
- CSV (SpreadSheet)
- Test Method
- 번역

## CSV (SpreadSheet)
### CSV.Parse
```cs
var context = Resources.Load<TextAsset>("Enemy.csv").text;
List<List<string>> csv = CSV.Parse(context);
```
csv형태의 파일을 2차원 배열 형태로 변환시킵니다.


### CSV.DeserializeToList\<T\>
```cs
var context = Resources.Load<TextAsset>("Enemy.csv").text;
List<Enemy> enemies = CSV.DeserializeToList<Enemy>(context);
```
csv데이터를 받아 형식에 맞춰 대상의 타입으로 파싱합니다.<br/>
형식: <br/>
|Name|Hp|Desc|
|:---:|:---:|:---:|
|**<span style="color:Tomato">String</span>**|**<span style="color:Tomato">Int32</span>**|**<span style="color:Tomato">String</span>**|
|Slime|10|Sticky|
|Orc|20|Big|
|Dragon|100|Strong|

1) 데이터 명
2) 데이터 타입 (기본 타입의 경우 System내에 있는 걸로 표기해야합니다. 
 ex: <span style="color:Tomato">int</span> <span style="color:lime">Int32</span>, <span style="color:Tomato">bool</span> <span style="color:lime">Boolean</span> 등, </br>
 또한 직접 만든 `class`, `Enum`의 경우 `Namespace`까지 포함해서 작성해야합니다.)
3) 데이터들

형태로 구성되어야합니다. 또한 파싱하기 위해선 대상 클래스에 열 이름에 해당하는 `Set Property`가 존재해야합니다.

### CSV.GenerateCode
```cs
var context = Resources.Load<TextAsset>("Enemy.csv").text;
CSV.GenerateCode("Enemy", context);
```
데이터를 기반으로 새로운 `class`를 생성합니다. 생성된 클래스는 `Scripts/AutoCSVOutputScript/(이름).cs`으로 저장됩니다.<br/>
이건 CSV Window를 이용해 더 쉽게 사용할 수 있으니 참고바랍니다. 

### SpreadSheet.Load
```cs
List<List<string>> data = SpreadSheet.LoadData(path, sheet, apikey);
```
- path <br/>
  `https://docs.google.com/spreadsheets/d/XXXX/edit?gid=YYYY`에서
  `XXXX`의 내용을 입력해주면 됩니다.
- sheet <br/>
  외부 SpreadSheet에서 받아올 시트명입니다. (ex: Enemy, Job, Item등)
- apikey<br/>
  Google Cloud Console에서 발급받을 수 있는 키입니다.

스프레드 시트에서 데이터를 받아오는 함수이나 ApiKey공개되기 쉬운 등의 문제의 여지가 있기 때문에 아래의 `SmartLoad`를 쓰는 것을 권장합니다.

### SpreadSheet.SmartLoad\<T\>
```cs
SpreadSheet.SmartLoad<Enemy>("Enemy", "Data");
```
위 예제는 `List<Enemy>`를 반환합니다. 
에디터라면 SpreadSheet에서 `Enemy`시트의 데이터를 받아서 `Resources/Data/Enemy.csv`로 저장합니다.(파일은 `Resources` 내부에 있어야합니다.)
또한 런타임에는 먼저 저장해둔 csv파일을 참조합니다. 또한 `CSV window`에서 에디터에서도 매번 저장 여부를 정할 수 있습니다. <br/>
<b>에디터 상에서 필요한 내용인 Path 지정은 CSV Window에 작성합니다. 무조건 아래에 있는 CSV Window 내부의 요소를 채운 뒤에 사용하세요.</b>

### CSV Window
- 여는 법<br/>
  유니티 메뉴 > windows > Utilities > CSV
- Spread Sheet Path<br/>
  `https://docs.google.com/spreadsheets/d/XXXXXXXXXX/edit?gid=YYYY`
  XXXX의 내용을 입력해주면 됩니다.
- Env File Path<br/>
  local경로로 `Assets`이하의 있는 파일을 선택합니다. 그 파일의 내용을 읽어 api키로 사용합니다.<br/>
  env.txt로 값을 넣었다면 아래와 같이 파일을 만들어야합니다.
  > `Assets/env.txt` <br/>
  > Api키 (Cloud console에서 발급받은 키)
- Load data on play<br/>
  `SmartLoad`에서 함수가 매 플레이마다 데이터를 Spread sheet에서 따올지 여부를 결정합니다.
- SaveDataPath<br/>
  자동으로 데이터가 저장될 경로입니다. 또한 기본 경로가 `Assets/`이기 때문에 `SmartLoad`를 쓰기 위해서는 `Resources` 하위 폴더를 선택하는 것을 추천드립니다.
- TargetSheet<br/>
  `SmartLoad`를 쓰지 않는다면 배열에 가져올 시트의 이름을 적고 아래 `Save`버튼을 통해 데이터를 `SaveDataPath`로 지정한 경로에 저장힙니다. <br/>
  혹은 `GenerateClass`를 눌러 해당하는 간단한 형태의 데이터 타입을 생성할 수 있습니다.
- TypeSheet<br/>
  |`TypeSheetExample`||||||
  |:---:|:---:|:---:|:---:|:---:|:---:|
  |Int32|-2147483648|~|2147483647|||
  |Boolean|true|false||||
  |String||||||
  |Rarerity|Normal|Rare|SuperRare|UltraRare|Legend|
  |Direction2x|None|Up|Down|Left|Right|

  예를 들어 이런 타입에 대해 정의하는 테이블이 존재한다고 가정해봅시다.
  이 시트의 이름을 `TypeSheet`에 작성하고 `GenerateEnum`버튼을 누르면 `Scripts/AutoCSVOutputScript/Enums.cs`에 원래 존재하는 타입이 아닌 enum들이 전부 저장되게 됩니다. 또한 이름을 2x로 끝나게 작성하면 flag enum으로 생성하며 각 요소는 하나의 플래그를 맡게됩니다. (예시에서 Direction2x의 요소들이 각각 `0`, `1`, `2`, `4`, `8`이 됩니다.)
