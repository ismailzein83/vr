// include plug-ins
var gulp = require('gulp');
var uglify = require('gulp-uglifyjs');
var del = require('del');


var config = {
    //Include all js files
    src: ['Client/Constants/*.js', 'Client/Views/*.js', 'Client/Services/*.js'],
    srcview: ['Client/Views/*.html'],
    projectName: 'Queueing'
}

// Synchronously delete the output file(s)
gulp.task('clean', function () {
    del.sync([config.projectName + '/Dist/' + config.projectName + '.min.js', config.projectName + '/Dist/' + config.projectName + '.min.js.map', config.projectName + '/Views']);
});

gulp.task('copy-view', function () {
    return gulp.src(config.srcview).pipe(gulp.dest(config.projectName + '/Views'));
});

// Combine and minify all files from the app folder
gulp.task('scripts', ['clean', 'copy-view'], function () {

    return gulp.src(config.src)
      .pipe(uglify(config.projectName + '.min.js', {
          outSourceMap: true
      }))
      .pipe(gulp.dest(config.projectName + '/Dist/'));
});

//Set a default tasks
gulp.task('default', ['scripts'], function () { });
