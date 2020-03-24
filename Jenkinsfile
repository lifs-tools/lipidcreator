node {
    def artifactoryServer = Artifactory.server 'LipidomicsExternal'
    def emailOnSuccess = "${jobEmailRecipients?:''}"
    def emailOnFailure = "${jobEmailRecipients?:''}"
    def gitRepo = "lifs-tools/lipidcreator"
    def gitHoster = "github.com"
    def gitUrl = "https://${gitHoster}/${gitRepo}"
    def gitUserName = "${jobGitUserName?:''}"
    def gitUserEmail = "${jobGitUserEmail?:''}"
    def gitUserCredentialsId = "${jobGitUserCredentials?:''}"
    def dockerRegistryUrl = "${jobDockerRegistryUrl?:''}"
    def dockerRegistryCredentialsId = "${jobDockerRegistryCredentialsId?:''}"
    def dockerBuildImage = "${jobDockerBuildImage?:''}"
    def afUploadSpec = """{
    "files": [
        {
            "pattern": "LipidCreator/bin/x64/Release/LipidCreator.zip",
            "target": "libs-release-local/de/isas/lipidomics/lipidcreator/${BUILD_NUMBER}/LipidCreator-${BUILD_NUMBER}.zip",
            "props": "artifactId:lipidcreator;groupId:de.isas.lipidomics;type:zip;version:${BUILD_NUMBER}",
            "recursive": "false",
            "flat" : "true",
            "regexp": "false"
        },
        {
            "pattern": "LipidCreator/bin/x64/Release/LipidCreator.zip",
            "target": "libs-release-local/de/isas/lipidomics/lipidcreator/${BUILD_NUMBER}/LipidCreator.zip",
            "props": "artifactId:lipidcreator;groupId:de.isas.lipidomics;type:zip;version:${BUILD_NUMBER}",
            "recursive": "false",
            "flat" : "true",
            "regexp": "false"
        }
    ]
}"""

    stage('Build & Test') {
        docker.withRegistry(dockerRegistryUrl, dockerRegistryCredentialsId) {
            docker.image(dockerBuildImage).inside {
                try {
                    stage 'Checkout'
                    def scmVars = checkout([$class: 'GitSCM', branches: [[name: '*/master']],
			extensions: scm.extensions + [[$class: 'WipeWorkspace']],
                        userRemoteConfigs: [[credentialsId: gitUserCredentialsId, url: gitUrl]]])
		    script {
			env.GIT_COMMIT = scmVars.GIT_COMMIT
			env.GIT_BRANCH = scmVars.GIT_BRANCH
                    }
                    stage 'Build'
                    sh 'export PATH="$PATH:/bin/:/sbin/:/usr/bin/:/usr/sbin/" && /usr/bin/msbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64 /p:BuildNumber=${BUILD_NUMBER}'
                    stage 'Test'
                    sh 'export PATH="$PATH:/bin/:/sbin/:/usr/bin/:/usr/sbin/" && cd LipidCreator && make tests && make runtest'
                    stage('Publish') {
                        def buildInfo = artifactoryServer.upload spec: afUploadSpec, failNoOp: true
                        artifactoryServer.publishBuildInfo buildInfo
                    }
                    stage 'Tag'
                    withCredentials([usernamePassword(credentialsId: gitUserCredentialsId, passwordVariable: 'GIT_PASSWORD', usernameVariable: 'GIT_USERNAME')]) {
                        sh("git config user.email '${jobGitUserEmail}'")
                        sh("git config user.name '${jobGitUserName}'")
                        sh("git tag -a '${BUILD_NUMBER}' -m 'Automatic tag for successful build number ${BUILD_NUMBER} from commit ${GIT_COMMIT} on branch ${GIT_BRANCH}'")
                        script {
                            env.encodedPass=URLEncoder.encode("${GIT_PASSWORD}", "UTF-8")
                            env.gitHoster=gitHoster
                            env.gitRepo=gitRepo
                        }
                        sh('git push https://${GIT_USERNAME}:${encodedPass}@${gitHoster}/${gitRepo} --tags')
                    }
                    stage 'Notify'
                    mail to: emailOnSuccess,
                         subject: "LipidCreator build succeeded: ${currentBuild.fullDisplayName}",
                         body: "Artifacts have been deployed to Artifactory, build tag was pushed to ${gitUrl}/releases/tag/${BUILD_NUMBER}."
                } catch(e) {
                    mail to: emailOnFailure,
                         subject: "LipidCreator build failed: ${currentBuild.fullDisplayName}",
                         body: """Please check the job errors:
                                  ${e}
                                """
                    throw e
                } finally {
                }
            }
        }
    }
}
