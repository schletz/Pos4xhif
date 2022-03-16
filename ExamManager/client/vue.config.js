const path = require('path');

const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  outputDir: path.resolve("../api/ExamManager.Api/wwwroot"),  
  transpileDependencies: true
})
