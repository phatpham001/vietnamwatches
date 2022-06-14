# vietnamwatches
Path frist run: "localhost:44375"
If you want the project to work,
first go to path:"..\MyClass\App.config" and path:"..\VietnamWatches\Web.config"
to change the content:
"<connectionStrings>
		<add name="StrConnect" connectionString="Data Source=(-Change here-);Initial Catalog=VietnamWatches01;Integrated Security=True" providerName="System.Data.SqlClient" />
	</connectionStrings>"
Next:
The password is encrypted in MD5 format, pay attention to create the first admin user with Accsess=1 to launch the admin page with the path "localhost:44375/Admin"
