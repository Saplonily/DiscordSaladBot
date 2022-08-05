# 指令解析系统

### 指令系统结构示例
- `salad`公共指令集  
    - `hello`指令
    - `age`指令
    - `check_ip`指令 *&lt;ip>*
    - `check_weather`指令 *&lt;city name> &lt;parameters>*
- `salad`独立指令集
    - `tictoc`指令集  
        - `place`指令 *&lt;width> &lt;height>*  
        - `start`指令  
        - `end`指令  
    - `countdown`指令集  
        - `new`指令 *&lt;time /s> &lt;countdown name>*

### 示例指令:
> `/salad tictoc place 1 1`  

被解析为`["salad","tictoc","place","1","1"]`
首先去公共根指令集去寻找叫`salad`的指令集  
找到了继续寻找叫`tictoc`的指令集  
在`tictoc`找不到`place`的指令集
所以调用place指令,并传递参数`(1,1)`  
如果`salad`公共指令集找不到  
那么寻找叫`salad`的独立指令集