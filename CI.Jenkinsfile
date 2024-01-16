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
                mySonarOpts="/k:\"rosette-api-csharp-binding\" /d:sonar.host.url=${env.SONAR_HOST_URL} /d:sonar.login=${env.SONAR_AUTH_TOKEN}"

                if("${env.CHANGE_ID}" != "null"){
                    mySonarOpts = "$mySonarOpts /d:sonar.pullrequest.key=${env.CHANGE_ID}"
                } else {
                    mySonarOpts = "$mySonarOpts /d:sonar.branch.name=${env.BRANCH_NAME}"
                } 

                if ("${env.CHANGE_BRANCH}" != "null") {
                    mySonarOpts="$mySonarOpts /d:sonar.pullrequest.base=${env.CHANGE_TARGET} /d:sonar.pullrequest.branch=${env.CHANGE_BRANCH}"
                }

                sh "docker run --rm \
                       --pull always \
                       --volume ${sourceDir}:/source \
                       mono:6 \
                       bash -c \"echo && \
                             echo [INFO] Updating package manager database. && \
                             apt-get update -qq && \
                             echo && \
                             echo [INFO] Installing required OS packages. && \
                             apt-get -qq install unzip default-jre-headless -y > /dev/null && \
                             echo && \
                             echo [INFO] Updating CA Certs file to address expired Lets Encrypt certificate. && \
                             echo [INFO] https://github.com/KSP-CKAN/CKAN/wiki/SSL-certificate-errors#removing-expired-lets-encrypt-certificates && \
                             sed -i 's,^mozilla/DST_Root_CA_X3.crt\$,!mozilla/DST_Root_CA_X3.crt,' /etc/ca-certificates.conf && \
                             echo && \
                             echo [INFO] Running update-ca-certificates && \
                             update-ca-certificates && \
                             echo && \
                             echo [INFO] Running cert-sync && \
                             cert-sync /etc/ssl/certs/ca-certificates.crt && \
                             echo && \
                             echo [INFO] Setting up Sonar Scanner && \
                             mkdir -p /opt/sonar-scanner && \
                             pushd /opt/sonar-scanner && \
                             curl --silent --output sonar-scanner.zip --location https://github.com/SonarSource/sonar-scanner-msbuild/releases/download/5.15.0.80890/sonar-scanner-msbuild-5.15.0.80890-net46.zip && \
                             unzip -q sonar-scanner.zip && \
                             chmod a+x /opt/sonar-scanner/sonar-scanner-*/bin/* && \
                             pushd /source && \
                             echo && \
                             echo [INFO] Running Sonar Scanner Begin && \
                             mono /opt/sonar-scanner/SonarScanner.MSBuild.exe begin ${mySonarOpts} && \
                             echo && \
                             echo [INFO] Restoring the solution. && \
                             nuget restore rosette_api.sln && \
                             echo && \
                             echo [INFO] Running msbuild Release configuration. && \
                             msbuild /p:Configuration=Release rosette_api.sln /t:Rebuild && \
                             echo && \
                             echo [INFO] Running Sonar Scanner End && \
                             mono /opt/sonar-scanner/SonarScanner.MSBuild.exe end /d:sonar.login=\"${env.SONAR_AUTH_TOKEN}\" && \
                             echo && \
                             echo [INFO] Running unit tests && \
                             mono ./packages/NUnit.Console.3.0.1/tools/nunit3-console.exe ./rosette_apiUnitTests/bin/Release/rosette_apiUnitTests.dll && \
                             echo && \
                             echo [INFO] Re-permission files for cleanup. && \
                             chown -R jenkins:jenkins /source\""

                             // TODO:  Finish coverage data gathering for Sonar.
                             ///opt/maven-basis/bin/mvn --batch-mode clean install sonar:sonar $mySonarOpts\""
                             //pushd /tmp
                             //dotcover_version=2022.3.1
                             //curl --silent --location --output dotcover.tar.gz https://download.jetbrains.com/resharper/dotUltimate.2022.3.1/JetBrains.dotCover.CommandLineTools.linux-x64.2022.3.1.tar.gz
                             //tar xzf dotcover.tar.gz
            }
        }
        postToTeams(true)
    } catch (e) {
        currentBuild.result = "FAILED"
        postToTeams(false)
        throw e
    }
}

def postToTeams(boolean success) {
    def webhookUrl = "${env.TEAMS_PNC_JENKINS_WEBHOOK_URL}"
    def color = success ? "#00FF00" : "#FF0000"
    def status = success ? "SUCCESSFUL" : "FAILED"
    def message = "*" + status + ":* '${env.JOB_NAME}' - [${env.BUILD_NUMBER}] - ${env.BUILD_URL}"
    office365ConnectorSend(webhookUrl: webhookUrl, color: color, message: message, status: status)
}
