# StringyDropdown
Unity classes for a dropdown drawer using a scriptable object as the data source.

## Usage

1. Add the StringyDropdown folder to you projects Assets folder.
2. Create a StringyStrings asset in your project hierarchy by going to 
     Create > StringyDropdown > New ...
3. Add some strings to the new file.
4. In a component class, add the property attribute above an string variable:

>    [StringyDropdown.StringyDropdown]
>    public string fauxEnum;

## Options

If you rename the DefaultStringyStrings asset (or have multiple), you can reference a 
specific asset by using the attribute argument FileName, like this:

>    [StringyDropdown.StringyDropdown(FileName = "OtherStringyFilename")]

