## Info

A visual comparison tool that displays your diff right in JIRA!

## Installation

### Pre-reqs & Notes

- The library was built to use SauceLabs or a Se2 Grid, but can easily be adapted to other hosts like BrowserStack. Check the BrowserManager class out!
- ImageMagick.NET runs on Windows, Linux, and MacOS at the time of this writing, so this should work cross-platform.
- The docker compose file spins up a SauceConnect proxy instance, allowing for internal sites to be diff'd. This can be disabled, and I will add instructions how when I have time... whoops.

### TestCaseManager

The tool uses a TestCaseManager. Essentially, the test case manager is what directs the browser to your relevant environments. Since we are using the tool across multiple clients and projects, we needed a way to manage the different "requirements" for taking the screenshots:

- Some pages require username/password
- Some sites have modals that need to be closed
- Some pages need accordions or other components interacted with prior to screenshots being taken

Since these varied so much, the easiest solution was to utilize TestCaseManagers based on the JIRA Project (the front part of the JIRA ticket number). 

Take a look at the ExampleTestCaseManager class provided and customize it to your liking. Once you are complete, don't forget to add your own to the NNAHUtils' RunJiraDiff function. This function is the basic orchestration for our multi-site capturing, and contains the logic that decides which TestCaseManager will be used for a given test case.

### JIRA

Set up as a [JIRA WebHook](https://developer.atlassian.com/server/jira/platform/webhooks/).

In our workflow, we have the WebHook have no "default" triggers, and instead trigger the WebHook via a [post function](https://confluence.atlassian.com/adminjiracloud/advanced-workflow-configuration-776636620.html#Advancedworkflowconfiguration-optionalpostfunctionsOptionalpostfunctions) on specific workflow transitions.

You will also need to create a local JIRA user with permission to read relevant tickets, upload attachments, and add comments.

As seen in the JIRAUtils class, the tool looks for a "URLs" field in JIRA. Without this piece of data, the tool would have no idea what URLs it is comparing.

### Docker

Install docker and docker-compose... [Here](https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-16-04) is a good tutorial for Ubuntu 16.04. I have included part of it below:


```
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
sudo apt-get update
apt-cache policy docker-ce

sudo apt-get install -y docker-ce
sudo systemctl status docker

sudo usermod -aG docker ${USER}
su - ${USER}
id -nG

docker run hello-world
```

### Docker Compose

```
sudo curl -L https://github.com/docker/compose/releases/download/1.19.0/docker-compose-`uname -s`-`uname -m` -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
docker-compose --version
```

If docker-compose is not found:

```
sudo ln -sf /usr/local/bin/docker-compose /usr/bin/docker-compose
```

### Visual Compare

Replace the variables below with your own! Remember to replace ens160 in the ifconfig command with your interface name. Alternatively, you can replace that entire command with your IP Address.

```
git clone ssh://git@bitbucket.criticalmass.com:7999/qa/qa-tools-visual.git

export CMQA_VISUAL_GRID_USER=sauce_labs_username
export CMQA_VISUAL_GRID_KEY="sauce_labs_api_key"
export CMQA_VISUAL_GRID_HOST="http://ondemand.saucelabs.com:80/wd/hub"

export CMQA_VISUAL_JIRA_HOST="https://jira_url"
export CMQA_VISUAL_JIRA_USER="some_jira_user"
export CMQA_VISUAL_JIRA_KEY="some_jira_password"

export CMQA_VISUAL_RABBIT_HOST=$(sudo ifconfig ens160 | sed -En 's/127.0.0.1//;s/.*inet (addr:)?(([0-9]*\.){3}[0-9]*).*/\2/p')
export CMQA_VISUAL_RABBIT_USER="a_username_for_this_tool_internally"
export CMQA_VISUAL_RABBIT_PASS="a_password_for_this_tool_internally"

docker-compose up -d
```

## Thoughts & Stuff...

I wrote this for internal use, but thought it could easily be adapted. That is why you see some language like CM, NNAH, and the likes around, as these are internal acronyms (nothing private, hold your horses).

If you have any questions or suggestions, I am 100% open to hearing them and doing what I can!

