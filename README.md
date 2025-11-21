# LearnQuickTyping
An application to learn how to type fast and without mistakes.



## C-SHARP Naming Conventions

This project follows standard C# naming conventions:

### **PascalCase** - Used for:
- **Namespaces**: `LearnQuickTyping.App.Views`
- **Classes**: `Main`, `ContentPage`  
- **Methods**: `DisplayRandomWord()`, `CalculateWordPerMinute()`
- **Properties**: `PracticeWord.Text`, `InputText.Text`
- **Constants/Readonly Fields**: `PracticeWords`

### **camelCase** - Used for:
- **Private Fields**: `startTime`, `isTiming`, `elapsedTime`
- **Method Parameters**: `sender`, `e`, `characterCount`
- **Local Variables**: `random`, `index`, `currentElapsed`

### **Other Conventions:**
- **Boolean Prefix**: `isTiming` (using "is" prefix for boolean fields)
- **Code Regions**: `#region Time Related Methods` (for code organization)
- **Modern Hungarian Notation**: `realTimeTimer` (limited use for UI controls)

The code consistently follows Microsoft's C# coding guidelines, ensuring readability and maintainability.



## XAML Naming Conventions

This XAML file follows MAUI/XAML naming conventions:

### **x:Name Attribute** - Used for:
- **UI Elements**: `PracticeWord`, `InputText`, `SecondsResult`, `WordsPerMinuteResult`, `TextResult`
- **Naming Pattern**: PascalCase for all named controls

### **Event Handlers** - Used for:
- **Completed Event**: `OnEntryDone` (matches C# method name)
- **TextChanged Event**: `StartTiming` (matches C# method name)

### **Properties and Attributes**:
- **x:Class**: `LearnQuickTyping.App.Views.Main` (matches C# class namespace and name)
- **Title**: `LearnQuickTyping` (PascalCase for page title)
- **Control Properties**: `Padding`, `Spacing`, `VerticalOptions`, `HorizontalOptions`, `Text`, `Placeholder` etc.

### **Layout Structure**:
- **Root Container**: `ContentPage`
- **Layout Container**: `VerticalStackLayout`
- **Child Elements**: `Label`, `Entry` controls with consistent naming

The XAML follows standard MAUI conventions with clear separation between UI definition and code-behind logic.



## Git Strategy
### Currently using = Gitflow.
Any features to the repository should be started as branches coming off the Development branch and be prefixed with feature/. \
Any patches for production should be applied directly to Master AND Development and be prefixed with hotfix/. \
All branches should be associated with a pull request for merging back into Development or Master branches. \
For non-trivial commits (or in general), having someone else approve your pull request is preferred.

### Workflow for new feature branch
1. Start on Development branch (git checkout development)
2. Pull recent changes (git pull)
3. Create new branch (git checkout -b feature/[feature-name])
4. Add changes (git status to see changes, git add . to add all changes)
5. Commit changes (git commit -m 'commit message')
6. Push to Github (git push origin feature/[feature-name])
7. Once the local feature branch is pushed up to Github, initate a pull request to have it included into the main Development branch. Delete feature branch after pull request is accepted.
