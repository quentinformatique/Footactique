import jsPDF from 'jspdf';
import { TeamComposition, PlayerPosition } from '../types/api';

function hexToRgb(hexColor: string | undefined): { r: number; g: number; b: number } {
  const fallback = { r: 59, g: 130, b: 246 }; // Tailwind blue-500 as default
  if (!hexColor) {
    return fallback;
  }
  let hex = hexColor.trim();
  if (hex.startsWith('#')) {
    hex = hex.slice(1);
  }
  if (hex.length === 3) {
    hex = hex.split('').map((c) => c + c).join('');
  }
  if (hex.length !== 6) {
    return fallback;
  }
  const r = parseInt(hex.substring(0, 2), 16);
  const g = parseInt(hex.substring(2, 4), 16);
  const b = parseInt(hex.substring(4, 6), 16);
  if (Number.isNaN(r) || Number.isNaN(g) || Number.isNaN(b)) {
    return fallback;
  }
  return { r, g, b };
}

function drawPitch(doc: jsPDF, x: number, y: number, width: number, height: number) {
  // Outer pitch
  doc.setDrawColor(255, 255, 255);
  doc.setFillColor(34, 197, 94); // green-ish background underlay (not full page)
  doc.setLineWidth(0.5);
  // Background
  doc.setFillColor(34, 197, 94);
  doc.rect(x, y, width, height, 'F');
  // Border
  doc.setDrawColor(255, 255, 255);
  doc.rect(x, y, width, height);

  // Mid line
  doc.line(x + width / 2, y, x + width / 2, y + height);

  // Center circle
  const centerX = x + width / 2;
  const centerY = y + height / 2;
  const centerCircleRadius = Math.min(width, height) * 0.07;
  doc.circle(centerX, centerY, centerCircleRadius);

  // Penalty boxes (simplified proportions)
  const boxWidth = width * 0.16;
  const boxHeight = height * 0.5;
  doc.rect(x, y + (height - boxHeight) / 2, boxWidth, boxHeight);
  doc.rect(x + width - boxWidth, y + (height - boxHeight) / 2, boxWidth, boxHeight);
}

function drawPlayers(doc: jsPDF, players: PlayerPosition[], pitch: { x: number; y: number; w: number; h: number }) {
  const radius = Math.max(3.5, Math.min(pitch.w, pitch.h) * 0.02); // in mm
  doc.setLineWidth(0.5);

  for (const player of players) {
    const px = pitch.x + (player.x * pitch.w);
    // Y is normalized with 0 = bottom, 1 = top in the model; invert to draw
    const py = pitch.y + ((1 - player.y) * pitch.h);

    const { r, g, b } = hexToRgb(player.color);
    doc.setFillColor(r, g, b);
    doc.setDrawColor(255, 255, 255);
    doc.circle(px, py, radius, 'FD');

    // Number or initial
    const label = player.number !== undefined && player.number !== null
      ? String(player.number)
      : (player.playerName || '').charAt(0).toUpperCase();
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(8);
    // Center text roughly
    doc.text(label, px, py + 0.8, { align: 'center' as any });

    // Player name under the circle
    doc.setTextColor(0, 0, 0);
    doc.setFontSize(7);
    doc.text(player.playerName, px, py + radius + 3.5, { align: 'center' as any });
  }
}

export class PdfExportService {
  public static async exportCompositionAsPdf(composition: TeamComposition): Promise<void> {
    // Page 1: Landscape full-pitch with players
    const doc = new jsPDF({ unit: 'mm', format: 'a4', orientation: 'landscape' });
    const landscapeWidth = doc.internal.pageSize.getWidth();
    const landscapeHeight = doc.internal.pageSize.getHeight();
    const margin = 8;

    // Compute pitch to fill as much as possible with standard aspect 105x68
    const pitchAspect = 105 / 68;
    const availableWidth1 = landscapeWidth - margin * 2;
    const availableHeight1 = landscapeHeight - margin * 2;
    let pitchWidth1 = availableWidth1;
    let pitchHeight1 = pitchWidth1 / pitchAspect;
    if (pitchHeight1 > availableHeight1) {
      pitchHeight1 = availableHeight1;
      pitchWidth1 = pitchHeight1 * pitchAspect;
    }
    const pitchX1 = margin + (availableWidth1 - pitchWidth1) / 2;
    const pitchY1 = margin + (availableHeight1 - pitchHeight1) / 2;

    // Draw full pitch and players
    drawPitch(doc, pitchX1, pitchY1, pitchWidth1, pitchHeight1);
    drawPlayers(doc, composition.players || [], { x: pitchX1, y: pitchY1, w: pitchWidth1, h: pitchHeight1 });

    // Small footer with title
    doc.setTextColor(17, 24, 39);
    doc.setFontSize(11);
    const footerText = (composition.name || 'Schéma tactique');
    doc.text(footerText, landscapeWidth / 2, landscapeHeight - 4, { align: 'center' as any });

    // Page 2: Portrait with details (title, formation, description, players list)
    doc.addPage('a4', 'portrait');
    const pageWidth = doc.internal.pageSize.getWidth();
    const pageHeight = doc.internal.pageSize.getHeight();
    const portraitMargin = 14;

    // Header
    doc.setFontSize(20);
    doc.setTextColor(17, 24, 39);
    doc.text(composition.name || 'Schéma tactique', portraitMargin, portraitMargin + 6);

    doc.setFontSize(11);
    doc.setTextColor(75, 85, 99);
    const subtitleParts: string[] = [];
    if (composition.formation) {
      subtitleParts.push(`Formation: ${composition.formation}`);
    }
    const generatedAt = new Date().toLocaleString('fr-FR');
    subtitleParts.push(`Exporté: ${generatedAt}`);
    doc.text(subtitleParts.join('  •  '), portraitMargin, portraitMargin + 14);

    // Optional description
    let cursorY = portraitMargin + 24;
    if (composition.description) {
      doc.setFontSize(11);
      doc.setTextColor(31, 41, 55);
      const descriptionLines = doc.splitTextToSize(composition.description, pageWidth - portraitMargin * 2);
      doc.text(descriptionLines, portraitMargin, cursorY);
      cursorY += descriptionLines.length * 5 + 6;
    }

    // Players list
    doc.setFontSize(14);
    doc.setTextColor(17, 24, 39);
    doc.text('Joueurs', portraitMargin, cursorY);
    cursorY += 7;

    doc.setFontSize(10);
    doc.setTextColor(31, 41, 55);
    const lineHeight = 5;
    const players = (composition.players || []).slice();
    for (const player of players) {
      const numberPart = player.number ? `#${player.number} ` : '';
      const positionPart = player.position ? ` – ${player.position}` : '';
      const row = `${numberPart}${player.playerName}${positionPart}`;
      doc.text(row, portraitMargin, cursorY);
      cursorY += lineHeight;
      if (cursorY > pageHeight - portraitMargin) {
        doc.addPage('a4', 'portrait');
        cursorY = portraitMargin;
      }
    }

    const safeName = (composition.name || 'schema').replace(/[^a-zA-Z0-9-_]+/g, '_');
    doc.save(`${safeName}.pdf`);
  }
}

