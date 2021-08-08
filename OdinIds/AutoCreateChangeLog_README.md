# AutoCreateChangelog

> 1. 修改package.json文件内的版本号信息（version）

> 2. npm init -y 初始化

> 3. npm i 安装对应npm包

> 4. 依次执行 npm run  release 和 npm run changelog 即可生成 CHANGELOG.MD 文件

```json
{
	"version": "1.0.19",
	"scripts": {
		"release": "standard-version",
		"changelog": "conventional-changelog -i CHANGELOG.md -s -r 0"
	},
	"devDependencies": {
		"better-than-before": "^1.0.0",
		"chai": "^4.2.0",
		"commitizen": "^3.1.1",
		"compare-func": "^1.3.2",
		"conventional-changelog-cli": "^2.0.21",
		"conventional-changelog-core": "^3.2.2",
		"coveralls": "^3.0.4",
		"cz-conventional-changelog": "^2.1.0",
		"git-dummy-commit": "^1.3.0",
		"github-url-from-git": "^1.5.0",
		"istanbul": "^0.4.5",
		"jscs": "^3.0.7",
		"jshint": "^2.10.2",
		"mocha": "*",
		"q": "^1.5.1",
		"shelljs": "^0.8.3",
		"standard-version": "^9.3.0",
		"through2": "^3.0.1"
	},
	"config": {
		"commitizen": {
			"path": "cz-conventional-changelog"
		}
	}
}
```