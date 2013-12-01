// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/**
 * @name: S7.6_A2.2_T1;
 * @section: 7.6;
 * @assertion: IdentifierPart :: UnicodeCombiningMark (any character in the Unicode categories "Mn", "Mc");
 * @description: UnicodeCombiningMark :: Non-spacing mark (Mn).
 * Complex test with eval;
*/

//CHECK
Mn = [[0x0300, 0x036F], [0x0483, 0x0487], [0x0591, 0x05BD], [0x05BF, 0x05BF], [0x05C1, 0x05C2], [0x05C4, 0x05C5], [0x05C7, 0x05C7], [0x0610, 0x061A], [0x064B, 0x065E], [0x0670, 0x0670], [0x06D6, 0x06DC], [0x06DF, 0x06E4], [0x06E7, 0x06E8], [0x06EA, 0x06ED], [0x0711, 0x0711], [0x0730, 0x074A], [0x07A6, 0x07B0], [0x07EB, 0x07F3], [0x0901, 0x0902], [0x093C, 0x093C], [0x0941, 0x0948], [0x094D, 0x094D], [0x0951, 0x0954], [0x0962, 0x0963], [0x0981, 0x0981], [0x09BC, 0x09BC], [0x09C1, 0x09C4], [0x09CD, 0x09CD], [0x09E2, 0x09E3], [0x0A01, 0x0A02], [0x0A3C, 0x0A3C], [0x0A41, 0x0A42], [0x0A47, 0x0A48], [0x0A4B, 0x0A4D], [0x0A51, 0x0A51], [0x0A70, 0x0A71], [0x0A75, 0x0A75], [0x0A81, 0x0A82], [0x0ABC, 0x0ABC], [0x0AC1, 0x0AC5], [0x0AC7, 0x0AC8], [0x0ACD, 0x0ACD], [0x0AE2, 0x0AE3], [0x0B01, 0x0B01], [0x0B3C, 0x0B3C], [0x0B3F, 0x0B3F], [0x0B41, 0x0B44], [0x0B4D, 0x0B4D], [0x0B56, 0x0B56], [0x0B62, 0x0B63], [0x0B82, 0x0B82], [0x0BC0, 0x0BC0], [0x0BCD, 0x0BCD], [0x0C3E, 0x0C40], [0x0C46, 0x0C48], [0x0C4A, 0x0C4D], [0x0C55, 0x0C56], [0x0C62, 0x0C63], [0x0CBC, 0x0CBC], [0x0CBF, 0x0CBF], [0x0CC6, 0x0CC6], [0x0CCC, 0x0CCD], [0x0CE2, 0x0CE3], [0x0D41, 0x0D44], [0x0D4D, 0x0D4D], [0x0D62, 0x0D63], [0x0DCA, 0x0DCA], [0x0DD2, 0x0DD4], [0x0DD6, 0x0DD6], [0x0E31, 0x0E31], [0x0E34, 0x0E3A], [0x0E47, 0x0E4E], [0x0EB1, 0x0EB1], [0x0EB4, 0x0EB9], [0x0EBB, 0x0EBC], [0x0EC8, 0x0ECD], [0x0F18, 0x0F19], [0x0F35, 0x0F35], [0x0F37, 0x0F37], [0x0F39, 0x0F39], [0x0F71, 0x0F7E], [0x0F80, 0x0F84], [0x0F86, 0x0F87], [0x0F90, 0x0F97], [0x0F99, 0x0FBC], [0x0FC6, 0x0FC6], [0x102D, 0x1030], [0x1032, 0x1037], [0x1039, 0x103A], [0x103D, 0x103E], [0x1058, 0x1059], [0x105E, 0x1060], [0x1071, 0x1074], [0x1082, 0x1082], [0x1085, 0x1086], [0x108D, 0x108D], [0x135F, 0x135F], [0x1712, 0x1714], [0x1732, 0x1734], [0x1752, 0x1753], [0x1772, 0x1773], [0x17B7, 0x17BD], [0x17C6, 0x17C6], [0x17C9, 0x17D3], [0x17DD, 0x17DD], [0x180B, 0x180D], [0x18A9, 0x18A9], [0x1920, 0x1922], [0x1927, 0x1928], [0x1932, 0x1932], [0x1939, 0x193B], [0x1A17, 0x1A18], [0x1B00, 0x1B03], [0x1B34, 0x1B34], [0x1B36, 0x1B3A], [0x1B3C, 0x1B3C], [0x1B42, 0x1B42], [0x1B6B, 0x1B73], [0x1B80, 0x1B81], [0x1BA2, 0x1BA5], [0x1BA8, 0x1BA9], [0x1C2C, 0x1C33], [0x1C36, 0x1C37], [0x1DC0, 0x1DE6], [0x1DFE, 0x1DFF], [0x20D0, 0x20DC], [0x20E1, 0x20E1], [0x20E5, 0x20F0], [0x2DE0, 0x2DFF], [0x302A, 0x302F], [0x3099, 0x309A], [0xA66F, 0xA66F], [0xA67C, 0xA67D], [0xA802, 0xA802], [0xA806, 0xA806], [0xA80B, 0xA80B], [0xA825, 0xA826], [0xA8C4, 0xA8C4], [0xA926, 0xA92D], [0xA947, 0xA951], [0xAA29, 0xAA2E], [0xAA31, 0xAA32], [0xAA35, 0xAA36], [0xAA43, 0xAA43], [0xAA4C, 0xAA4C], [0xFB1E, 0xFB1E], [0xFE00, 0xFE0F], [0xFE20, 0xFE26]];

errorCount = 0;
count = 0;
for (indexI = 0; indexI < Mn.length; indexI++) {
  for (indexJ = Mn[indexI][0]; indexJ <= Mn[indexI][1]; indexJ++) {
    try {
      var identifier = "$" + String.fromCharCode(indexJ);
      var hex = decimalToHexString(indexJ);
      eval("var " + identifier + "=1");
      if (eval(identifier + "===1") !== true) {
        $ERROR('#' + hex + ' ');
        errorCount++;
      }
    } catch (e) {
      $ERROR('#' + hex + ' ');
      errorCount++;
    }
    count++;
  }
}

if (errorCount > 0) {    
  $ERROR('Total error: ' + errorCount + ' bad Unicode character in ' + count);
}

function decimalToHexString(n) {
  n = Number(n);
  var h = "";
  for (var i = 3; i >= 0; i--) {
    if (n >= Math.pow(16, i)) {
      var t = Math.floor(n / Math.pow(16, i));
      n -= t * Math.pow(16, i);
      if ( t >= 10 ) {
        if ( t == 10 ) { h += "A"; }
        if ( t == 11 ) { h += "B"; }
        if ( t == 12 ) { h += "C"; }
        if ( t == 13 ) { h += "D"; }
        if ( t == 14 ) { h += "E"; }
        if ( t == 15 ) { h += "F"; }
      } else {
        h += String(t);
      }
    } else {
      h += "0";
    }
  }
  return h;
}
