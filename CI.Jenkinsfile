node ("docker-light") {
    def sourceDir = pwd()
    try {
        stage("Clean up") {
            step([$class: 'WsCleanup'])
        }
        stage("Checkout Code") {
            checkout scm
        }
        stage("Build & Test") {
            withSonarQubeEnv {
                // TODO:  Remove if we branch references work, otherwise, pass these to the exe somehow.
                //if ("${env.CHANGE_BRANCH}" != "null") {
                //     mySonarOpts="$mySonarOpts -Dsonar.pullrequest.key=${env.CHANGE_ID} -Dsonar.pullrequest.base=${env.CHANGE_TARGET} -Dsonar.pullrequest.branch=${env.CHANGE_BRANCH}"
                //}
                //echo("Sonar Options are: $mySonarOpts")
                // https://github.com/KSP-CKAN/CKAN/wiki/SSL-certificate-errors#removing-expired-lets-encrypt-certificates
                // Reference for sed command and cert sync steps.
                sh "docker run --rm \
                       --pull always \
                       --volume ${sourceDir}:/source \
                       mono:6 \
                       bash -c \"apt-get update && \
                             apt-get install unzip default-jre -y && \
                             sed -i 's,^mozilla/DST_Root_CA_X3.crt\$,!mozilla/DST_Root_CA_X3.crt,' /etc/ca-certificates.conf && \
                             update-ca-certificates && \
                             cert-sync /etc/ssl/certs/ca-certificates.crt && \
                             mkdir -p /opt/sonar-scanner && \
                             pushd /opt/sonar-scanner && \
                             sonar_version=5.9.2.58699 && \
                             curl --silent --output sonar-scanner.zip --location https://github.com/SonarSource/sonar-scanner-msbuild/releases/download/\${sonar_version}/sonar-scanner-msbuild-\${sonar_version}-net46.zip && \
                             unzip sonar-scanner.zip && \
                             chmod a+x /opt/sonar-scanner/sonar-scanner-*/bin/* && \
                             pushd /source && \
                             mono /opt/sonar-scanner/SonarScanner.MSBuild.exe begin /k:\"rosette-api-csharp-binding\" /d:sonar.login=\"${env.SONAR_AUTH_TOKEN}\" /d:sonar.host.url=\"${env.SONAR_HOST_URL}\" && \
                             nuget restore rosette_api.sln && \
                             msbuild /p:Configuration=Release rosette_api.sln /t:Rebuild && \
                             mono /opt/sonar-scanner/SonarScanner.MSBuild.exe end /d:sonar.login=\"${env.SONAR_AUTH_TOKEN}\" && \
                             mono ./packages/NUnit.Console.3.0.1/tools/nunit3-console.exe ./rosette_apiUnitTests/bin/Release/rosette_apiUnitTests.dll\""
                             // TODO:  Finish coverage data gathering for Sonar.
                             ///opt/maven-basis/bin/mvn --batch-mode clean install sonar:sonar $mySonarOpts\""
                             //pushd /tmp
                             //dotcover_version=2022.3.1
                             //curl --silent --location --output dotcover.tar.gz https://download.jetbrains.com/resharper/dotUltimate.\${dotcover_version}/JetBrains.dotCover.CommandLineTools.linux-x64.\${dotcover_version}.tar.gz
                             //tar xzf dotcover.tar.gz
            }
        }
        slack(true)
    } catch (e) {
        currentBuild.result = "FAILED"
        slack(false)
        throw e
    }
}

def slack(boolean success) {
    def color = success ? "#00FF00" : "#FF0000"
    def status = success ? "SUCCESSFUL" : "FAILED"
    def message = status + ": Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"
    slackSend(color: color, channel: "#p-n-c_jenkins", message: message)
}