# Contributing to Timed Silver

We are excited that you're interested to help us move forward and improve Timed Silver.

The flow when contributing to Timed Silver is as follows:

1. Add issue (bug or new feature)
2. Wait for issue to be tagged `you-take-it`
3. Commit your changes and rebase
4. Create a pull request

Note that your contributions must be your own work and licensed under the same terms as Timed Silver.

When contributing to this repository, please first discuss the change you wish to make via the issue
tracker or any other method with the owners of this repository before making a change. 

Please note we have a code of conduct, please follow it in all your interactions with the project.

## Reporting bugs

First, search the issue tracker to see if the bug is already there. When you are sure, please add defects in the issue tracker.

Please use markdown to format your code blocks with [syntax highlighting](https://help.github.com/articles/github-flavored-markdown/#syntax-highlighting) to make it easier for everyone to read. 

You could also use services like https://gist.github.com to share code snippets.

A bug report should include:

- used platform and tools version
- used Timed Silver version
- description of the issue
- steps to reproduce the issue

## Suggesting new features

Please add it directly in the issue tracker in the same way as bug reports.

## Create your own fork

1. Log in to GitHub and open the [timed-silver](https://github.com/silviubogan/timed-silver/) origin repository. Click the "Fork" button to create your own fork of the repository.
2. Create a clone on your local system: `git clone https://github.com/yourusername/timed-silver.git`

## Branch

The repository contains one important branch:

- `master` - the main branch with the latest development changes

Create a branch for the bugfix/feature you want to work on: `git branch bugfix-some-error`

Checkout the branch: `git checkout bugfix-some-error`

## Commit your changes

Before you start committing, please define your [full name](https://help.github.com/articles/setting-your-username-in-git/) and [e-mail](https://help.github.com/articles/setting-your-email-in-git/) address in git.

Please follow the style of the other messages in the commit history. Explain the intentions for the change and include the issue number. Include the issue number in the commit message, e.g. "#9945".

To commit your changes, use the git command: `git commit -m <msg>`

Finally, push your commits: `git push`

## Requirements for a pull request

- Update the CHANGELOG.md file
- Update the CONTRIBUTORS file if you are not already listed there

## Common rules

To prevent the issue (backlog) list to grow indefinitely, an issue will be closed when you are not responding to requests by a project developer.
We need your help, so we will not close an issue by desire. But when you are not answering to a request by for a month we will close the issue.
If this happens in accident, feel free to re-open the issue...

The issue reporter can close the issue because he/she verified the fix/change

In case the reporter does not respond to a request, a project developer is allowed to close the issue (after one month).

Someone can take over the issue from the reporter, when:

- he/she can reproduce the issue
- has some sample code
- has more time/information to assist in the resolution
- responds to requests by a project developer