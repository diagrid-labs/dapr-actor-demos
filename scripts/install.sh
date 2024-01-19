dapr uninstall
dapr init
dotnet build ./BasicActorSamples/BasicActorSamples.sln
dotnet build ./EvilCorpDemo/EvilCorpDemo.sln
cd ./EvilCorpDemo/EvilCorp.FrontEnd
npm install
cd ../..