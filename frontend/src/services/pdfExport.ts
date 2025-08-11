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
    // A4 in mm
    const doc = new jsPDF({ unit: 'mm', format: 'a4' });
    const pageWidth = doc.internal.pageSize.getWidth();
    const pageHeight = doc.internal.pageSize.getHeight();
    const margin = 12;

    // Header: title and meta
    doc.setFontSize(18);
    doc.setTextColor(17, 24, 39); // gray-900
    doc.text(composition.name || 'Schéma tactique', margin, margin + 6);

    doc.setFontSize(11);
    doc.setTextColor(75, 85, 99); // gray-600
    const subtitleParts: string[] = [];
    if (composition.formation) {
      subtitleParts.push(`Formation: ${composition.formation}`);
    }
    const generatedAt = new Date().toLocaleString('fr-FR');
    subtitleParts.push(`Exporté: ${generatedAt}`);
    doc.text(subtitleParts.join('  •  '), margin, margin + 14);

    // Description (if present)
    let cursorY = margin + 22;
    if (composition.description) {
      doc.setFontSize(11);
      doc.setTextColor(31, 41, 55); // gray-800
      const descriptionLines = doc.splitTextToSize(composition.description, pageWidth - margin * 2);
      doc.text(descriptionLines, margin, cursorY);
      cursorY += descriptionLines.length * 5 + 4;
    }

    // Pitch area calculation
    const availableWidth = pageWidth - margin * 2;
    const availableHeight = pageHeight - cursorY - margin - 70; // leave space for players list at bottom
    const pitchAspect = 105 / 68; // standard football pitch
    let pitchWidth = availableWidth;
    let pitchHeight = pitchWidth / pitchAspect;
    if (pitchHeight > availableHeight) {
      pitchHeight = availableHeight;
      pitchWidth = pitchHeight * pitchAspect;
    }
    const pitchX = margin + (availableWidth - pitchWidth) / 2;
    const pitchY = cursorY;

    // Draw pitch background rectangle in a faint green (underlay)
    drawPitch(doc, pitchX, pitchY, pitchWidth, pitchHeight);

    // Draw players on pitch
    drawPlayers(doc, composition.players || [], { x: pitchX, y: pitchY, w: pitchWidth, h: pitchHeight });

    // Section: Players list
    let listY = pitchY + pitchHeight + 10;
    if (listY < pageHeight - margin - 10) {
      doc.setFontSize(14);
      doc.setTextColor(17, 24, 39);
      doc.text('Joueurs', margin, listY);
      listY += 6;

      doc.setFontSize(10);
      doc.setTextColor(31, 41, 55);
      const lineHeight = 5;
      const players = (composition.players || []).slice();
      for (const player of players) {
        const numberPart = player.number ? `#${player.number} ` : '';
        const positionPart = player.position ? ` – ${player.position}` : '';
        const row = `${numberPart}${player.playerName}${positionPart}`;
        doc.text(row, margin, listY);
        listY += lineHeight;
        if (listY > pageHeight - margin) {
          doc.addPage();
          listY = margin;
        }
      }
    }

    const safeName = (composition.name || 'schema').replace(/[^a-zA-Z0-9-_]+/g, '_');
    doc.save(`${safeName}.pdf`);
  }
}

