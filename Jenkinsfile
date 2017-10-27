def upstreamEnv = new EnvVars()
node {
    def SOURCEDIR = pwd()
    try {
        //if the current build is running by another we begin to getting variables
        def upstreamCause = currentBuild.rawBuild.getCause(Cause$UpstreamCause)
        if (upstreamCause) {
            def upstreamJobName = upstreamCause.properties.upstreamProject
            def upstreamBuild = Jenkins.instance
                                    .getItemByFullName(upstreamJobName)
                                    .getLastBuild()
            upstreamEnv = upstreamBuild.getAction(EnvActionImpl).getEnvironment()
        }
        def altUrl = upstreamEnv.ALT_URL

        stage("Clean up") {
            step([$class: 'WsCleanup'])
        }
        stage("Checkout Code") {
            checkout scm
        }
        stage("Test with Docker") {
            echo "${altUrl}"
            def useUrl = ("${altUrl}" == "null") ? "${env.BINDING_TEST_URL}" : "${env.ALT_URL}"
            withEnv(["API_KEY=${env.ROSETTE_API_KEY}", "ALT_URL=${useUrl}"]) {
                sh "docker run --rm -e API_KEY=${API_KEY} -e ALT_URL=${ALT_URL} -v ${SOURCEDIR}:/source rosetteapi/docker-csharp"
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
    slackSend(color: color, channel: "#rapid", message: message)
}