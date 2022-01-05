---
title: Deploying to GitHub Pages
abstract: Publishing content to GitHub Pages using Azure DevOps pipelines
date: 2022-01-05
tags: Blogging,GitHub,Azure DevOps
---

# Deploying to GitHub Pages

[GitHub Pages](https://docs.github.com/en/pages) is a way to freely host a static website. It has a [few restrictions](https://docs.github.com/en/pages/getting-started-with-github-pages/about-github-pages#prohibited-uses) but is ideal for a developer to host a personal website. It requires a [GitHub Account](https://github.com/signup) but as developers we already have one of those, right?


1. Create a repository called ```<user>.github.io``` - this will be where content is published.

1. Disable the default [Jekyll](https://jekyllrb.com/) site generation by adding an empty ```.nojekyll``` text file in the root of the repository.

1. Create a [personal access token](https://github.com/settings/tokens) to be used by Azure DevOps

1. Create a Variable Group and Variable in the [Azure DevOps Pipelines -> Library](https://docs.microsoft.com/en-us/azure/devops/pipelines/library) so that the token can be securely consumed by any build pipelines - making sure that the variable is marked as *secure*. Make sure any pipelines that need to access the group are set up in the "Pipeline Permissions" tab.




