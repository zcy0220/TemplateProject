set GEN_CLIENT=dotnet ..\Tools\Luban.ClientServer\Luban.ClientServer.dll

%GEN_CLIENT% -j cfg --^
 -d Defines\__root__.xml ^
 --input_data_dir Datas ^
 --output_data_dir ..\..\Assets\ArtPack\Pack\Datas\Tables ^
 --output_code_dir ..\..\Assets\Scripts\GameMain\Modules\TableModule ^
 --gen_types code_cs_unity_bin,data_bin ^
 -s all
pause