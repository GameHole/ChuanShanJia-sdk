@echo off
cd %~dp0
git tag -d 1.0.0
git push origin :refs/tags/1.0.0
