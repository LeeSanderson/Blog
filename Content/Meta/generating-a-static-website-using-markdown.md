---
title: Generating a Static Website Using Markdown
abstract: How to build a static website from a bunch of Markdown documents using C#, Markdig, and RazorEngine.NetCore
date: 2021-05-28
tags: Blogging,Markdown,CSharp,Markdig
---

# Generating a Static Website Using Markdown

In my [Blogging with GitHub and Markdown](blogging-with-github-and-markdown.md) article, I discussed using Markdown to author content for your blog. In this follow up article, I will use C#, [Markdig](https://github.com/xoofx/markdig), and [RazorEngine.NetCore](https://github.com/fouadmess/RazorEngine) to convert the Markdown content to HTML so that it can be published on to the Internet.

```mermaid
graph TD;
    A (Markdown Source) -> B [Markdig] -> C (Raw HTML)
```


