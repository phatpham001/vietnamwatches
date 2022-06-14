# vietnamwatches
Path frist run: "localhost:44375"<br />
If you want the project to work,<br />
first go to path:"..\MyClass\App.config" and path:"..\VietnamWatches\Web.config"<br />
to change the content:<br />
"<connectionStrings>
		<add name="StrConnect" connectionString="Data Source=(-Change here-);Initial Catalog=VietnamWatches01;Integrated Security=True" providerName="System.Data.SqlClient" />
	</.connectionStrings>"<br />
Next:<br />
The password is encrypted in MD5 format, pay attention to create the first admin user with Accsess=1 to launch the admin page with the path "localhost:44375/Admin"<br />
