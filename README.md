ISBD Project is home budget (spending-management fashion) tracker project for classes in engineering of database systems at the Wrocław University of Technology.
Project uses sqlite library and is written in C# with WPF. 

To create new view:
1. Create new Page (add controlls or what you want)
2. Create states:
	2.1 Create UI state
	2.2 (Optional) Create logic state
3. Create Connector interface
4. Implement connector in *PageName*.cs file
5. Make UIState inherit from ConnectorState<*Connector*, *PageName*>
